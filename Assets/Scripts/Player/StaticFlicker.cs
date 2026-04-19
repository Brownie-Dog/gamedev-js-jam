using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class StaticFlicker : MonoBehaviour
{
    private RawImage _rawImage;

    void Awake()
    {
        _rawImage = GetComponent<RawImage>();
        Assert.IsNotNull(_rawImage);
    }

    void Update()
    {
        _rawImage.uvRect = new Rect(Random.value, Random.value, 1, 1);
    }
}