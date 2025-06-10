using UnityEngine;

public class RainController : MonoBehaviour
{
    private ParticleSystem _rain;
    private CloudsController _cc;
    public GeneralController _gc;

    void Start()
    {
        _rain = GetComponent<ParticleSystem>();
        _cc = FindAnyObjectByType<CloudsController>();
        //_gc = FindAnyObjectByType<GeneralController>();

        if( _rain == null || _cc == null || _gc == null)
        {
            Debug.Log("Error in Rain Controller");
            return;
        }

        RainStop();
    }

    // Update is called once per frame
    void Update()
    {
        //if (_cc.skyIsChanging())
        //{
        //    rainStart();
        //}

    }

    public void RainStart()
    {
        _rain.Play();
    }

    public void RainStop()
    {
        _rain.Stop();
    }

    public void RainChangeSettings(string mood)
    {
        var emission = _rain.emission;
        var main = _rain.main;

        int maxParticles = 0;
        int rateOverTime = 0;


        switch (mood)
        {
            case "normal":
                _rain.Stop();
                break;
            case "sad":
                maxParticles = 10000;
                rateOverTime = 100;
                break;
            case "stressed":
                maxParticles = 30000;
                rateOverTime = 10000;
                break;
            case "calm":
                _rain.Stop();
                break;
        }

        emission.rateOverTime = new ParticleSystem.MinMaxCurve(rateOverTime);
        main.maxParticles = maxParticles;
    }
}
