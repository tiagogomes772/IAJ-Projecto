using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Assets.Scripts.IAJ.Unity.Util;
using Assets.Scripts.IAJ.Unity.Movement;

public class PriorityManager : MonoBehaviour
{
    // WORLD
    public const float X_WORLD_SIZE = 55;
    public const float Z_WORLD_SIZE = 32.5f;
    public int MAX_BOIDS = 20;

    // BOIDS
    public const float MAX_LOOK_AHEAD = 16.0f;
    public const float AVOID_MARGIN = 8.0f;
    public const float MAX_SPEED = 20.0f;
    public const float MAX_ACCELERATION = 40.0f;
    public const float DRAG = 0.1f;
    public const float MAX_FAN_ANGLE = MathConstants.MATH_2PI; // verificar este valor...
    public const float MAX_SLOW_RADIUS = 8;
    public const float MAX_STOP_RADIUS = 2;
    public const float MAX_SEPARATION_FACTOR = 30;
    public const float MAX_RADIUS = 10;
    public const float MIN_SPEED = 4.0f;
    public float WANDER_RADIUS = 0;
    public float WANDER_OFFSET = 3;
    public float WHISKER_ANGLE = 30;
    public float WHISKER_LOOK_AHEAD = 3;

    // WEIGHTS
    public float AVOID_OBSTACLE_WEIGHT = 50.0f;
    public float SEPARATION_WEIGHT = 5.0f;
    public float COHESION_WEIGHT = 5.0f;
    public float FLOCK_MATCHING_WEIGHT = 15.0f;
    public float STRAIGHT_AHEAD_WEIGHT = 20.0f;
    public float WANDER_WEIGHT = 0.0f;

    private Camera MainCamera { get; set; }

    private DynamicCharacter RedCharacter { get; set; }

    private List<DynamicCharacter> Characters { get; set; }

    private bool MousePressed { get; set; }

    private Vector3 Point { get; set; }

    // Use this for initialization
    void Start () 
	{
        this.MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        var textObj = GameObject.Find ("InstructionsText");
		if (textObj != null) 
		{
			textObj.GetComponent<Text>().text = 
				"Instructions\n\n" +
				"Click wherever you want"; 
		}

		var redObj = GameObject.Find ("Red");

	    this.RedCharacter = new DynamicCharacter(redObj)
	    {
	        Drag = DRAG,
	        MaxSpeed = MAX_SPEED
	    };
        
	    var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

	    this.Characters = this.CloneSecondaryCharacters(redObj, MAX_BOIDS, obstacles);
	    this.Characters.Add(this.RedCharacter);

        //initialize all but the last character (because it was already initialized as the main character)
	    foreach (var character in this.Characters)
	    {
	        this.InitializeCharacter(character, obstacles);
	    }
	}

    private void InitializeCharacter(DynamicCharacter character, GameObject[] obstacles)
    {
        PriorityMovement tempPriorityPrimary = new PriorityMovement
        {
            Character = character.KinematicData
        };

        BlendedMovement tempBlended = new BlendedMovement
        {
            Character = character.KinematicData
        };

        foreach (var obstacle in obstacles)
        {

            DynamicAvoidObstacle avoidObstacleMovement = new DynamicAvoidObstacle(obstacle)
            {
                MaxAcceleration = MAX_ACCELERATION,
                avoidMargin = AVOID_MARGIN,
                maxLookAhead = MAX_LOOK_AHEAD,
                whiskerAngle = WHISKER_ANGLE,
                whiskerLookAhead = WHISKER_LOOK_AHEAD,
                Character = character.KinematicData,
                Target = new KinematicData(),
                MovementDebugColor = Color.magenta
            };
            tempPriorityPrimary.Movements.Add(avoidObstacleMovement);
            //tempBlended.Movements.Add(new MovementWithWeight(avoidObstacleMovement, AVOID_OBSTACLE_WEIGHT));
        }

        FlockVelocityMatching flockVelocityMatching = new FlockVelocityMatching()
        {
            Character = character.KinematicData,
            Target = new KinematicData(),
            MovementDebugColor = Color.magenta,
            fanAngle = MAX_FAN_ANGLE,
            flock = Characters,
            MaxAcceleration = MAX_ACCELERATION,
            radius = MAX_RADIUS
        };
        Cohesion cohesionMovement = new Cohesion()
        {
            Character = character.KinematicData,
            Target = new KinematicData(),
            MovementDebugColor = Color.magenta,
            flock = Characters,
            fanAngle = MAX_FAN_ANGLE,
            MaxAcceleration = MAX_ACCELERATION,
            maxSpeed = MAX_SPEED,
            name = "Cohesion",
            radius = MAX_RADIUS,
            slowRadius = MAX_SLOW_RADIUS,
            stopRadius = MAX_STOP_RADIUS
        };
        Separation separationMovement = new Separation()
        {
            character = character.KinematicData,
            flock = Characters,
            maxAcceleration = MAX_ACCELERATION,
            radius = MAX_STOP_RADIUS,
            separationFactor = MAX_SEPARATION_FACTOR
        };

        DynamicStraightAhead straightAhead = new DynamicStraightAhead
        {
            Character = character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MovementDebugColor = Color.yellow
        };
        DynamicWander wander = new DynamicWander
        {
            Character = character.KinematicData,
            MovementDebugColor = Color.yellow,
            MaxAcceleration = MAX_ACCELERATION,
            WanderRate = MathConstants.MATH_PI_2,
            WanderRadius = WANDER_RADIUS,
            WanderOffset = WANDER_OFFSET
        };
        DynamicFleeRadius flee = new DynamicFleeRadius
        {
            Character = character.KinematicData,
            MovementDebugColor = Color.yellow,
            MaxAcceleration = MAX_ACCELERATION,
            radius = 10
        };

        #region Blended Movements

        tempBlended.Movements.Add(new MovementWithWeight(cohesionMovement, COHESION_WEIGHT));
        tempBlended.Movements.Add(new MovementWithWeight(separationMovement, SEPARATION_WEIGHT));
        tempBlended.Movements.Add(new MovementWithWeight(flockVelocityMatching, FLOCK_MATCHING_WEIGHT));
        tempBlended.Movements.Add(new MovementWithWeight(straightAhead, STRAIGHT_AHEAD_WEIGHT));
        tempBlended.Movements.Add(new MovementWithWeight(wander, WANDER_WEIGHT));
        tempBlended.Movements.Add(new MovementWithWeight(flee, 0.0f));
        #endregion

        tempPriorityPrimary.Movements.Add(tempBlended);

        /*foreach (var otherCharacter in this.Characters)
        {
            if (otherCharacter != character)
            {
                //TODO: add your avoidCharacter movement here
                var avoidCharacter = new DynamicAvoidCharacter(otherCharacter.KinematicData)
                {
                    Character = character.KinematicData,
                    MaxAcceleration = MAX_ACCELERATION,
                    AvoidMargin = AVOID_MARGIN,
                    MovementDebugColor = Color.cyan
                };
                #region Blended Movements
                tempBlended.Movements.Add(new MovementWithWeight(avoidCharacter, 50f));
                #endregion
                //priority.Movements.Add(avoidCharacter);
            }
        }*/

        character.Movement = tempPriorityPrimary;
        character.Blended = tempBlended;
    }

    private List<DynamicCharacter> CloneSecondaryCharacters(GameObject objectToClone,int numberOfCharacters, GameObject[] obstacles)
    {
        var characters = new List<DynamicCharacter>();
        for (int i = 0; i < numberOfCharacters; i++)
        {
            var clone = GameObject.Instantiate(objectToClone);
            //clone.transform.position = new Vector3(30,0,i*20);
            clone.transform.position = this.GenerateRandomClearPosition(obstacles);
            var character = new DynamicCharacter(clone)
            {
                MaxSpeed = MAX_SPEED,
                Drag = DRAG
            };
            //character.KinematicData.orientation = (float)Math.PI*i;
            characters.Add(character);
        }

        return characters;
    }


    private Vector3 GenerateRandomClearPosition(GameObject[] obstacles)
    {
        Vector3 position = new Vector3();
        var ok = false;
        while (!ok)
        {
            ok = true;

            position = new Vector3(Random.Range(-X_WORLD_SIZE,X_WORLD_SIZE), 0, Random.Range(-Z_WORLD_SIZE,Z_WORLD_SIZE));

            foreach (var obstacle in obstacles)
            {
                var distance = (position - obstacle.transform.position).magnitude;

                //assuming obstacle is a sphere just to simplify the point selection
                if (distance < obstacle.transform.localScale.x + AVOID_MARGIN)
                {
                    ok = false;
                    break;
                }
            }
        }

        return position;
    }

	void Update()
	{
        if (Input.GetMouseButton(0))
        {
            var mousePos = Input.mousePosition;
            mousePos.z = this.MainCamera.transform.position.y;
            this.Point = this.MainCamera.ScreenToWorldPoint(mousePos);
            this.Point.Set(this.Point.x, 0.0f, this.Point.z);
            this.MousePressed = true;
        }

        foreach (var character in this.Characters)
	    {
	        this.UpdateMovingGameObject(character);
	    }
        this.MousePressed = false;
	}

    private void UpdateMovingGameObject(DynamicCharacter movingCharacter)
    {
        if (this.MousePressed)
        {
            MovementWithWeight movementW = movingCharacter.Blended.Movements.Find(obj => obj.Movement is DynamicFleeRadius);

            movementW.Movement.Target = new KinematicData { position = this.Point };
            movementW.Weight = 150.0f;
        }
        if (movingCharacter.Movement != null)
        {
            movingCharacter.Update();
            movingCharacter.KinematicData.ApplyWorldLimit(X_WORLD_SIZE,Z_WORLD_SIZE);
            movingCharacter.GameObject.transform.position = movingCharacter.Movement.Character.position;
        }
    }

}
