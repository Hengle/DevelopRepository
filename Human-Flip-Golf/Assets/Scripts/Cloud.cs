using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    Renderer myRenderer;

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            if (!myRenderer.isVisible && transform.position.x < 0)
            {
                var pos = transform.position;
                pos.x = Mathf.Abs(pos.x);
                transform.position = pos;
            }

            transform.Translate(transform.right * 0.1f * -1f);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

    }
}
