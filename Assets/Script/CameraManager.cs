using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]GameObject playerRef;
    Vector3 ref_Velocity = Vector3.zero;
        float smoothTime = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPOsition =  new Vector3(playerRef.transform.position.x, playerRef.transform.position.y, -10);
        // permet de pouvoir la lock plus tar, ducoup on parente pas dans unity pour avoir totalement la main dessus
        gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, targetPOsition, ref ref_Velocity, smoothTime);
        // SmoothDamp ralenti doucement la fin du mouvement 


    }
}
