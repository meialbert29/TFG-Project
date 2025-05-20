using UnityEngine;

public class RainController : MonoBehaviour
{
    ParticleSystem _rain;
    CloudsController _cc;

    void Start()
    {
        _rain = GetComponent<ParticleSystem>();
        _cc = FindAnyObjectByType<CloudsController>();

        if( _rain == null)
        {
            Debug.Log("Particle system not found");
            return;
        }
        else
        {
            Debug.Log("ps found");
        }

        rainStop();
    }

    // Update is called once per frame
    void Update()
    {
        if (_cc.skyIsChanging())
        {
            rainStart();
        }

    }

    public void rainStart()
    {
        _rain.Play();
    }

    public void rainStop()
    {
        _rain.Stop();
    }
}
