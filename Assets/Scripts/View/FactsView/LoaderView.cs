using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LoaderView : MonoBehaviour
{
    private Image _image;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private void OnValidate()
    {
        if (Application.isEditor)
        {
            _image = GetComponent<Image>();
        }
    }

    private async void OnEnable()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        await Spin(_cancellationTokenSource.Token);
    }

    private void OnDisable()
    {
        _cancellationTokenSource.Cancel();
    }

    private async UniTask Spin(CancellationToken cancellationToken)
    {
        var firstCall = true;
        while (!cancellationToken.IsCancellationRequested || gameObject.activeInHierarchy)
        {
            if (!firstCall)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
            }

            firstCall = false;
            _image.transform.localRotation = Quaternion.identity;
            _image.transform.DORotate(new Vector3(0f,0f,180f), 1f);
        }
    }
}
