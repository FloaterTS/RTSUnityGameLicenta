using UnityEngine;

public enum Formation
{
    FREE,
    FACING,
    //SQUARE,
    //DIAMOND,
    NONE
}

public class MovementManager : MonoBehaviour
{
    public static MovementManager instance;

    public Formation formationType;
    public float distanceMultiplier = 2f;
    public float unitSize = 1f;

    [HideInInspector]
    public bool heroInFrontOfAll = false;

    private SelectionManager unitSelection;
    private Terrain groundTerrain;

    private bool startFromMiddleLine = false;   // change for formation movement tuning
    private bool startFromMiddleColumn = false;  // change for formation movement tuning


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another unit movement manager present.");
    }

    void Start()
    {
        unitSelection = GetComponent<SelectionManager>();
        groundTerrain = GetComponent<GameManager>().mainTerrain;
    }

    public void MoveInFormation(Vector3 moveToSpot)
    {
        AssignPositions(GenerateFormation(moveToSpot));
    }

    void AssignPositions(Vector3[] formationPositions)
    {
        bool[] assignedSpots = new bool[formationPositions.Length];
        bool[] assignedUnits = new bool[formationPositions.Length]; //formationPositions has the same size as the selectedUnits list

        Vector3 currentMiddlePoint = FindMiddlePoint();

        int farthestSpotIndex;
        int closestUnitIndex;
        for(int i = 0; i < formationPositions.Length; i++)
        {
            if (formationType == Formation.FREE)
                farthestSpotIndex = FarthestSpotIndexFrom(formationPositions, assignedSpots, currentMiddlePoint);
            else
                farthestSpotIndex = i;

            closestUnitIndex = ClosestUnitIndexTo(formationPositions[farthestSpotIndex], assignedUnits);

            assignedSpots[farthestSpotIndex] = true;
            assignedUnits[closestUnitIndex] = true;

            unitSelection.selectedUnits[closestUnitIndex].MoveToLocation(formationPositions[farthestSpotIndex]);
        }
    }

    Vector3[] GenerateFormation(Vector3 mainSpot)
    {
        switch(formationType)
        {
            case Formation.FREE: return FreeFormation(mainSpot);
            case Formation.FACING: return SquareFormation(mainSpot, true);
            //case Formation.SQUARE: return SquareFormation(mainSpot, false);
            //case Formation.DIAMOND: return SquareFormation(mainSpot, false, 45f);
            case Formation.NONE: return NoFormation(mainSpot);
        }
        return null;
    }

    Vector3[] FreeFormation(Vector3 targetCenterSpot)
    {
        Vector3 originCenterPoint = FindMiddlePoint();

        Vector3 distanceDifference = targetCenterSpot - originCenterPoint;

        Vector3 targetUnitPosition;
        Vector3[] freeFormationPositions = new Vector3[unitSelection.selectedUnits.Count];
        for(int i = 0; i < unitSelection.selectedUnits.Count; i++)
        {
            targetUnitPosition = unitSelection.selectedUnits[i].transform.position + distanceDifference;
            freeFormationPositions[i] = new Vector3(
                targetUnitPosition.x, groundTerrain.SampleHeight(targetUnitPosition), targetUnitPosition.z
                );
        }

        return freeFormationPositions;
    }

    Vector3[] SquareFormation(Vector3 targetCenterSpot, bool faceTargetPosition = true, float yRotation = 0f)
    {
        Vector3[] squareFormationPositions = new Vector3[unitSelection.selectedUnits.Count];

        Quaternion relativeRotation;
        if (faceTargetPosition)
            relativeRotation = Quaternion.Euler(
                0f, YAnglesFromPointToTargetPoint(FindMiddlePoint(), targetCenterSpot), 0f
                ); // Here we get the rotation on the y axis between the previous middle point and the new target spot
        else
            relativeRotation = Quaternion.Euler(0f, yRotation, 0f);

        // We turn the formation in a perfect square, then we add part of the remainder to the square sides 
        // as much as they fit, and afterwards, we put the ones left at the back of the pack.
        int currentPos = 0;
        int numberOfUnitsInSelection = unitSelection.selectedUnits.Count;
        //When heroes will be implemented
        /*if (unitSelection.heroSelected && heroInFrontOfAll && faceTargetPosition)
        {
            numberOfUnitsInSelection--;
            currentPos++;
        }*/
        int sideOfSquare = (int)Mathf.Sqrt(numberOfUnitsInSelection);
        int notInSquare = numberOfUnitsInSelection - sideOfSquare * sideOfSquare;
        int fullLines = sideOfSquare;
        int fullColumns = sideOfSquare + notInSquare / sideOfSquare;
        int behindFormation = notInSquare % sideOfSquare;
        int totalLines = behindFormation > 0 ? fullLines + 1 : fullLines;
        float offsetUpDown;
        float offsetLeftRight;

        Vector3 targetCenterSpotFlat = new Vector3(targetCenterSpot.x, 0f, targetCenterSpot.z);
        //Vector3 targetUnitPositionNR; Vector3 targetUnitPositionRotated;

        if (startFromMiddleLine)
            offsetUpDown = fullLines % 2 == 1 ? 0f : -0.5f; // we start assigning positions from the center outwards
        else
            offsetUpDown = (float)totalLines / 2 - 0.5f; //we start assigning positions starting from the front row
        for(int i = 0; i < fullLines; i++)
        {
            if(startFromMiddleColumn)
                offsetLeftRight = fullColumns % 2 == 1 ? 0f : -0.5f; // we start assigning position from the middle outwards
            else
                offsetLeftRight = (-1) * ((float)fullColumns / 2 - 0.5f); //we start assigning positions from left to right
                for (int j = 0; j < fullColumns; j++)
            {
                squareFormationPositions[currentPos] = UnitPositionInFormation(
                    targetCenterSpotFlat, relativeRotation, offsetUpDown, offsetLeftRight
                    );
                
                if(startFromMiddleColumn)
                    offsetLeftRight = offsetLeftRight < 0f ? -offsetLeftRight : -offsetLeftRight - 1f; 
                else
                    offsetLeftRight++;
                currentPos++;
            }
            if (startFromMiddleLine)
                offsetUpDown = offsetUpDown < 0f ? -offsetUpDown : -offsetUpDown - 1f;
            else
                offsetUpDown--;
        }
        // And now the ones behind who were not enough for a full line or column
        if (behindFormation > 0)
        {
            if (startFromMiddleLine) // else: it will already be at the right offset because it was lowered in the last for-loop iteration
                offsetUpDown = (-1) * ((float)totalLines / 2 - 0.5f);
            if(startFromMiddleColumn)
                offsetLeftRight = behindFormation % 2 == 1 ? 0f : -0.5f; // we start assigning position from the middle outwards
            else
                offsetLeftRight = (-1) * ((float)behindFormation / 2 - 0.5f); //we start assigning positions from left to right
            for (int j = 0; j < behindFormation; j++)
            {
                squareFormationPositions[currentPos] = UnitPositionInFormation(
                    targetCenterSpotFlat, relativeRotation, offsetUpDown, offsetLeftRight
                    );

                if(startFromMiddleColumn)
                    offsetLeftRight = offsetLeftRight < 0f ? -offsetLeftRight : -offsetLeftRight - 1f;
                else
                    offsetLeftRight++;
                currentPos++;
            }
        }
        //When heroes will be implemented use this for hero in front of all formation
        /*if(unitSelection.heroSelected && heroInFrontOfAll && faceTargetPosition)
        {
            offsetUpDown = (float)totalLines / 2 + 0.5f;
            offsetLeftRight = 0;

            squareFormationPositions[0] = UnitPositionInFormation(
                    targetCenterSpotFlat, relativeRotation, offsetUpDown, offsetLeftRight
                    );
        }*/

        return squareFormationPositions;
    }

    Vector3[] NoFormation(Vector3 targetCenterSpot)
    {
        Vector3[] noFormationPositions = new Vector3[unitSelection.selectedUnits.Count];
        for (int i = 0; i < unitSelection.selectedUnits.Count; i++)
            noFormationPositions[i] = targetCenterSpot;
        return noFormationPositions;
    }

    Vector3 FindMiddlePoint() // Gets the middle point between all the selected units
    {
        float minFormationX = unitSelection.selectedUnits[0].transform.position.x;
        float maxFormationX = minFormationX;

        float minFormationZ = unitSelection.selectedUnits[0].transform.position.z;
        float maxFormationZ = minFormationZ;

        foreach (Unit unit in unitSelection.selectedUnits)
        {
            minFormationX = Mathf.Min(minFormationX, unit.transform.position.x);
            maxFormationX = Mathf.Max(maxFormationX, unit.transform.position.x);

            minFormationZ = Mathf.Min(minFormationZ, unit.transform.position.z);
            maxFormationZ = Mathf.Max(maxFormationZ, unit.transform.position.z);
        }

        Vector3 middleSpot = new Vector3(
            (minFormationX + maxFormationX) / 2, 0f, (minFormationZ + maxFormationZ) / 2
            );

        return middleSpot;
    }

    // This function will return the angle between the vector starting from a center point and pointing upwards 
    // and another point (like numbers on a clock, the rotation starting at 0 degrees - at 12 o'clock)
    float YAnglesFromPointToTargetPoint(Vector3 originPoint, Vector3 targetPoint) //Relative to vector3.forward
    {
        float distToTarget = Vector3.Distance(originPoint, targetPoint);
        Vector3 differenceVector = targetPoint - originPoint;
        differenceVector.y = 0f;

        float angleBetween = Vector3.Angle(Vector3.forward * distToTarget, differenceVector);

        if (targetPoint.x > originPoint.x)
            return angleBetween;
        else
            return 360f - angleBetween;
    }

    Vector3 UnitPositionInFormation(Vector3 targetCenterSpotFlat, Quaternion relativeRotation, float offsetUpDown, float offsetLeftRight)
    {
        Vector3 targetUnitPositionNR = new Vector3(
                    targetCenterSpotFlat.x + offsetLeftRight * unitSize * distanceMultiplier,
                    0f,
                    targetCenterSpotFlat.z + offsetUpDown * unitSize * distanceMultiplier
                    );

        // Now we rotate the difference vector between the current unit position and the formation center
        // point based on the rotation obtained from comparing the target position to the previous position
        // or given from yRotation if faceRotation is set to false
        Vector3 targetUnitPositionRotated = targetCenterSpotFlat + relativeRotation * (targetUnitPositionNR - targetCenterSpotFlat);
        
        return new Vector3(targetUnitPositionRotated.x, groundTerrain.SampleHeight(targetUnitPositionRotated), targetUnitPositionRotated.z);
    }

    int FarthestSpotIndexFrom(Vector3[] formationPositions, bool[] assignedSpots, Vector3 relativeSpot)
    {
        float farthestSpotDistance = 0f;
        float distanceToSpot;
        int farthestSpotIndex = 0;
        for(int i = 0; i < assignedSpots.Length; i++)
        {
            if (assignedSpots[i])
                continue;
            distanceToSpot = Vector3.Distance(relativeSpot, formationPositions[i]);
            if(distanceToSpot > farthestSpotDistance)
            {
                farthestSpotDistance = distanceToSpot;
                farthestSpotIndex = i;
            }
        }
        return farthestSpotIndex;
    }

    int ClosestUnitIndexTo(Vector3 targetSpot, bool[] assignedUnits)
    {
        float closestUnitDistance = 1000f;
        float distanceToSpot;
        int closestUnitIndex = 0;
        for(int i = 0; i < assignedUnits.Length; i++)
        {
            if (assignedUnits[i])
                continue;
            distanceToSpot = Vector3.Distance(unitSelection.selectedUnits[i].transform.position, targetSpot);
            if (distanceToSpot < closestUnitDistance)
            {
                closestUnitDistance = distanceToSpot;
                closestUnitIndex = i;
            }
        }
        return closestUnitIndex;
    }

}
