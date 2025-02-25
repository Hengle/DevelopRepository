﻿using UnityEngine;

namespace Es.InkPainter.Sample
{
	[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
	public class CollisionPainter : MonoBehaviour
	{
		[SerializeField]
		private Brush brush = null;

		[SerializeField]
		private int wait = 3;

		private int waitCount;

		public void Awake()
		{
			GetComponent<MeshRenderer>().material.color = brush.Color;
		}

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.SphereCast(ray, 0.01f, out hit, Mathf.Infinity))
                {
                    if (waitCount < wait)
                        return;
                    waitCount = 0;
                    var canvas = hit.collider.GetComponent<InkCanvas>();
                    if (canvas != null)
                        canvas.Paint(brush, hit.point);
                }
            }
        }

        public void FixedUpdate()
		{
			++waitCount;
		}

		public void OnCollisionStay(Collision collision)
		{
			if(waitCount < wait)
				return;
			waitCount = 0;

			foreach(var p in collision.contacts)
			{
				var canvas = p.otherCollider.GetComponent<InkCanvas>();
				if(canvas != null)
					canvas.Paint(brush, p.point);
			}
		}
	}
}