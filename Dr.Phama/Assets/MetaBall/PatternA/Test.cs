using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject myObject;

    private IEnumerator Start()
    {
        while (true)
        {
            Instantiate(myObject,transform.position,transform.rotation);

            yield return new WaitForSeconds(0.1f);
        }

    }
}
