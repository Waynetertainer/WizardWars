using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public int rotationSpeed = 200;
    public Directions direction = Directions.YAxis;

    private Vector3 rotationAxis;

    private void Start()
    {
        switch (direction)
        {
            case Directions.YAxis:
                rotationAxis = Vector3.up;
                break;
            case Directions.XAxis:
                rotationAxis = Vector3.right;
                break;
            case Directions.ZAxis:
                rotationAxis = Vector3.forward;
                break;
        }
    }

    void Update () 
	{
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }

    public enum Directions
    {
        YAxis,
        XAxis,
        ZAxis
    };
}
