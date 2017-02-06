using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class LineController : MonoBehaviour
{
    private Transform _playerHand;
    private Vector3 _target;
    private Vector3 _targetMod;
    private LineRenderer _lineRenderer;
    private bool _visible;
    
    private float _showTimeMax = 0.05f;
    private float _showTimeMin = 0.01f;
    private float _showTimeCur;

    private float _hideTimeMax = 0.005f;
    private float _hideTimeMin = 0.001f;
    private float _hideTimeCur;

    private float _posRnd = 0.495f;

    void Start ()
    {
        _playerHand = this.transform;
        _target = Vector3.zero;
        _targetMod = Vector3.zero;
	    _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetPositions(new[] {_playerHand.position, _target + _targetMod });
        _visible = false;
    }
	
	// Update is called once per frame
	void Update ()
	{
        //TODO Alpha?
        //TODO Particles on the hand and on the hitpoiont
        if(!_visible) return;
	    UpdateLine(Time.deltaTime);
	}

    public void ShowLine(Vector3 target, Color color)
    {
        if(_visible) return;
        _visible = true;
        _target = target;
        _targetMod = new Vector3( Random.Range(-1*_posRnd, _posRnd), Random.Range(-1*_posRnd, _posRnd), 0.0f);
        _showTimeCur = Random.Range(_showTimeMin, _showTimeMax);
        _hideTimeCur = Random.Range(_hideTimeMin, _hideTimeMax);
        _lineRenderer.material.color = color;
    }

    public void HideLine()
    {
        _visible = false;
        _target = Vector3.zero;
        _lineRenderer.enabled = false;
    }

    private void UpdateLine(float deltaTime)
    {
        _showTimeCur -= deltaTime;

        if (_showTimeCur > 0f)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, _playerHand.position);
            _lineRenderer.SetPosition(1, _target + _targetMod);
            return;
        }

        _lineRenderer.enabled = false;
        _hideTimeCur -= deltaTime;

        if (_hideTimeCur > 0f) return;
        _showTimeCur = Random.Range(_showTimeMin, _showTimeMax);
        _hideTimeCur = Random.Range(_hideTimeMin, _hideTimeMax);
        _targetMod = new Vector3(Random.Range(-1 * _posRnd, _posRnd), Random.Range(-1 * _posRnd, _posRnd), 0f);
        //_lineRenderer.material.color = Random.ColorHSV();
        var c = _lineRenderer.material.color;
        _lineRenderer.material.color = new Color(c.r, c.g, c.b, Random.Range(0.1f, 0.8f));
    }
}
