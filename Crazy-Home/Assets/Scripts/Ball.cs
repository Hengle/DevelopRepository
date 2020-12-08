//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using DG.Tweening;

//public class Ball : Gimmick
//{
//    [SerializeField] GameObject ball;
//    [SerializeField] GameObject ballParent;

//    protected override void Start()
//    {
//        base.Start();
//    }

//    protected override void PlayPlayerAniation()
//    {
//        isAnimationWait = true;
//        ball.transform.parent = ballParent.transform;
//        ball.transform.localPosition = Vector3.zero;
//        player.PlayGimmickAnimation("Throw");
//    }

//    protected override void ActiveTrap()
//    {
//        base.ActiveTrap();
//        ball.transform.parent = null;
//        ball.transform.DOMove(enemy.transform.position.SetY(1.0f), 0.5f)
//           .OnStart(() => isAnimationWait = false)
//           .OnComplete(() => {
//               ball.transform.GetComponent<Rigidbody>().useGravity = true;
//               ball.transform.GetComponent<Collider>().isTrigger = false;
//               Invoke("Destroy", 2.0f);
//           });
//    }

//    private void Destroy()
//    {
//        var meshRenderer = ball.GetComponent<MeshRenderer>();
//        DOTween.To
//        (
//            () => 1.0f,
//            (alpha) =>
//            {
//                var material = meshRenderer.material;
//                Color color = material.color;
//                color.a = alpha;
//                meshRenderer.material.color = color;
//            },
//            0f,
//            1.5f
//        ).OnComplete(() => Destroy(ball));
//    }
//}
