using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    // Starting position for the parallax game project.
    Vector2 startingPosition;  

    // Start Z value of the parallax game object.
    float startingZ;

    // Update itself everyframe
    Vector2 canMoveSinceStart => (Vector2) cam.transform.position - startingPosition;

    float zDistanceFromTarget => transform.position.z - followTarget.transform.position.z;

    // If object is in front of target, use nearClipPlane. If behind target, use farClipPlane.
    float clippingPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane)); 

    float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippingPlane;

    // Start is called before the first frame update.
    void Start()
    {
        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        // When the target move, move the parallax object the same distance times a multiplier.
        Vector2 newPostion = startingPosition + canMoveSinceStart * parallaxFactor;

        // The X/Y position changed based on target travel speed times the parallax factor, but Z stay consistent .
        transform.position = new Vector3(newPostion.x, newPostion.y, startingZ);
    }
}
