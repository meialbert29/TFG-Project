using UnityEngine;

public class RainController : MonoBehaviour
{
    private ParticleSystem _rain;
    private CloudsController _cloudsController;
    public GeneralController _generalController;

    void Start()
    {
        _rain = GetComponent<ParticleSystem>();
        _cloudsController = FindAnyObjectByType<CloudsController>();
        //_gc = FindAnyObjectByType<GeneralController>();

        if( _rain == null || _cloudsController == null || _generalController == null)
        {
            Debug.Log("Error in Rain Controller");
            return;
        }

        RainStop();
    }

    // Update is called once per frame
    void Update()
    {

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
            case "calm":
                _rain.Stop();
                break;
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
            case "anxious":
                maxParticles = 50000;
                rateOverTime = 30000;
                break;
        }

        emission.rateOverTime = new ParticleSystem.MinMaxCurve(rateOverTime);
        main.maxParticles = maxParticles;
    }

    public void UpdateWind(float speed, Vector3 direction)
    {
        var forceOverLifetime = _rain.forceOverLifetime;
        forceOverLifetime.enabled = true;

        if(direction.x < 0)
            forceOverLifetime.x = new ParticleSystem.MinMaxCurve(-direction.x * speed *2);
        else
            forceOverLifetime.x = new ParticleSystem.MinMaxCurve(direction.x * speed * 2);

        forceOverLifetime.y = new ParticleSystem.MinMaxCurve(direction.y * speed);

        if(direction.z < 0)
            forceOverLifetime.z = new ParticleSystem.MinMaxCurve(-direction.z * speed * 2);
        else
            forceOverLifetime.z = new ParticleSystem.MinMaxCurve(direction.z * speed * 2);
    }

}
