using System.Collections;
using UnityEngine;

public class PulseToBeat : MonoBehaviour
{
    [SerializeField] bool _userTestBeat;
    [SerializeField] float _pulseSize = 1.15f;
    [SerializeField] float _returnSpeed = 5f;
    private Vector3 _startSize;
    private Material _mat;
    private Color _startCol;

    private void Start()
    {
        _startSize = transform.localScale;
        _mat = GetComponent<MeshRenderer>().material;
        _startCol = _mat.color;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _startSize, Time.deltaTime * _returnSpeed);
        _mat.color = Color.Lerp(_mat.color, Color.grey, Time.deltaTime * _returnSpeed);
    }

    public void Pulse()
    {
        transform.localScale = _startSize * _pulseSize;
    }

    public void Flash()
    {
        _mat.color = _startCol;
    }
}
