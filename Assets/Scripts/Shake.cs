using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public float duration = .5f, posMagnitude = .1f, rotMagnitude = 5;
    private float elapsed, x, y, z, xRot, yRot, zRot, startTime;
    private Vector3 prevPos, prevRot;
    private bool shaking = false;

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //        ShakeCam();
    //}

    public void ShakeCam()
    {
        if (shaking)
            return;
        shaking = true;
        StartCoroutine(Shaking(posMagnitude, rotMagnitude, 0));
    }

    public void ShakeCam(float delay)
    {
        if (shaking)
            return;
        shaking = true;
        StartCoroutine(Shaking(posMagnitude, rotMagnitude, delay));
    }

    public void ShakeCam(float posMag, float rotMag)
    {
        if (shaking)
            return;
        shaking = true;
        StartCoroutine(Shaking(posMag, posMag, 0));
    }

    private IEnumerator Shaking(float posMag, float rotMag, float delay)
    {
        yield return new WaitForSeconds(delay);
        prevPos = transform.localPosition;
        prevRot = transform.localEulerAngles;
        elapsed = 0f;
        startTime = Time.time;

        while(elapsed < duration)
        {
            x = Random.Range(-1f, 1f) * posMag;
            y = Random.Range(-1f, 1f) * posMag;
            z = Random.Range(-1f, 1f) * posMag;


            xRot = Random.Range(-1f, 1f) * rotMag;
            yRot = Random.Range(-1f, 1f) * rotMag;
            zRot = Random.Range(-1f, 1f) * rotMag;

            transform.localPosition = new Vector3(prevPos.x + x, prevPos.y + y, prevPos.z + z);
            transform.localEulerAngles = new Vector3(prevRot.x + xRot, prevRot.y + yRot, prevRot.z + zRot);
            elapsed = Time.time - startTime;
            yield return null;
        }

        transform.localPosition = prevPos;
        transform.localEulerAngles = prevRot;
        shaking = false;
    }


}
