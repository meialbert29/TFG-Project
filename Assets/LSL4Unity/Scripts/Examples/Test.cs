using UnityEngine;
using System;
using System.Linq;
using Assets.LSL4Unity.Scripts.AbstractInlets;
using MathNet.Numerics.IntegralTransforms;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.UI;

namespace Assets.LSL4Unity.Scripts.Examples
{
    public class ExampleFloatInlet : AFloatInlet
    {
        [SerializeField] private ScoreSystem scoreSystem;
        [SerializeField] private HerzsController herzsController;
        [SerializeField] private GeneralController generalController;

        public string lastSample = String.Empty;
        public int samplingRate = 256;
        private int bufferSize = 512; // samples number
        private int numChannels = 5;
        private Queue<float> eegBuffer = new Queue<float>(); // buffer
        public Renderer targetObjectRenderer;

        public string waveType;
        public float dominantFrequency;

        [SerializeField] private Text scoreText;
        [SerializeField] private Text pointsText;

        private double actualScore = 0;

        private Queue<string> waveBuffer = new Queue<string>(); // Store last 5 waves
        public string lastWaveType = ""; // last registered wave
        private bool waveChanged = false; // indicates if there was a wave change
        private float timeSinceLastScore = 0f; // time since last score
        private float scoreInterval = 10f; // interval to add points
        private int points = 0;

        private Dictionary<int, Queue<float>> channelBuffers = new Dictionary<int, Queue<float>>();
        private Dictionary<int, float> dominantFrequencies = new Dictionary<int, float>();

        float averageFrequency = 0f;

        public float AverageFrequency { get { return averageFrequency; } set { averageFrequency = value; } }

        public void StartMuseController()
        {
            for (int channel = 0; channel < numChannels; channel++)
            {
                channelBuffers[channel] = new Queue<float>();
            }
        }
        protected override void Process(float[] newSample, double timeStamp)
        {
            for (int channel = 0; channel < 5; channel++)
            {
                channelBuffers[channel].Enqueue(newSample[channel]);
                if (channelBuffers[channel].Count > bufferSize)
                {
                    channelBuffers[channel].Dequeue();
                }
            }

            bool allBuffersFull = true;

            for (int channel = 0; channel < numChannels; channel++)
            {
                if (channelBuffers[channel].Count != bufferSize)
                {
                    allBuffersFull = false;
                    break;
                }

            }
            // only analyze if all buffers are full
            if (allBuffersFull)
            {
                float[] dominantFrequencies = new float[numChannels];
                string[] waveTypes = new string[numChannels];

                // Analizar la frecuencia dominante por canal
                for (int i = 0; i < 5; i++)
                {
                    dominantFrequencies[i] = AnalyzeEEG(channelBuffers[i].ToArray());
                }

                dominantFrequency = dominantFrequencies.Average();


                for (int i = 0; i < numChannels; i++)
                {
                    waveTypes[i] = ClassifyBrainWave(channelBuffers[i].ToArray());
                }

                // create a dictionary to count how many times each wave appears
                Dictionary<string, int> waveCounts = new Dictionary<string, int>();

                // count the appearances
                foreach (string wave in waveTypes)
                {
                    if (!waveCounts.ContainsKey(wave))
                        waveCounts[wave] = 1;
                    else
                        waveCounts[wave]++;
                }

                // find the wave that appears the most
                string finalWave = "";
                int maxCount = 0;

                foreach (var wave in waveCounts)
                {
                    if (wave.Value > maxCount)
                    {
                        maxCount = wave.Value;
                        finalWave = wave.Key;
                    }
                }

                // finalWave is the dominant wave for the whole set
                waveType = finalWave;

                // add the detected wave to the buffer to check for consistency
                waveBuffer.Enqueue(waveType);
                if (waveBuffer.Count > 5)
                    waveBuffer.Dequeue();

                // check if the last waves are the same
                bool consistent = true;

                foreach (var wave in waveBuffer)
                {
                    if (wave != waveType)
                    {
                        consistent = false;
                        break;
                    }
                }

                if (consistent)
                {
                    if (waveType != lastWaveType)
                    {
                        scoreSystem.pointsEarnedText.enabled = false;
                        // if it changes to a new wave after another, add less
                        if (waveChanged)
                        {
                            points = 1;
                            actualScore += points;

                            scoreSystem.PointsEarned = 1;
                            scoreSystem.Score += points;
                        }
                        // if it changes to a new wave and stays, add more
                        else
                        {
                            points = 2;
                            scoreSystem.pointsEarnedText.enabled = true;
                            scoreSystem.PointsEarned = 2;
                            scoreSystem.Score += points;

                        }
                        waveChanged = true;
                        timeSinceLastScore = 0f; // reset time counter
                    }
                    else
                    {
                        timeSinceLastScore += Time.deltaTime;
                        // add points if wave stays the same for 10 secs
                        if (timeSinceLastScore >= scoreInterval)
                        {
                            scoreSystem.pointsEarnedText.enabled = true;
                            scoreSystem.PointsEarned = 20;
                            scoreSystem.Score += scoreSystem.PointsEarned;
                            timeSinceLastScore = 0f; // reset timer
                        }
                    }
                }
                else
                {
                    scoreSystem.pointsEarnedText.enabled = false;
                    waveChanged = false; // if wave is not consistent, wait for stable change
                    timeSinceLastScore = 0f; // reset timer if unstable
                }

                lastWaveType = waveType; // update last detected wave

                //ChangeObjectColor(waveType);
                herzsController.SetFrequency(dominantFrequency);
                ChangeGeneralMood(waveType);
                scoreSystem.UpdateScorePoints();
                scoreSystem.UpdatePointsEarned();
            }
        }

        private float AnalyzeEEG(float[] samples)
        {
            int N = samples.Length;
            Complex[] fftInput = new Complex[N];

            for (int i = 0; i < N; i++)
                fftInput[i] = new Complex(samples[i], 0);

            Fourier.Forward(fftInput, FourierOptions.NoScaling);

            float[] magnitudes = fftInput.Select(c => (float)c.Magnitude).ToArray();

            int maxIndex = Array.IndexOf(magnitudes, magnitudes.Max());
            float frequencyResolution = (float)samplingRate / N;
            return maxIndex * frequencyResolution;
        }

        private string ClassifyBrainWave(float[] samples)
        {
            // call a function that calculates how much energy each wave has
            var waveMagnitudes = GetWaveMagnitudes(samples);

            // find the wave with the highest energy (the dominant one)
            string dominantWave = null;
            float highestEnergy = 0f;

            // check each wave and its energy to find the highest
            foreach (var wave in waveMagnitudes)
            {
                if (wave.Value > highestEnergy)
                {
                    highestEnergy = wave.Value;
                    dominantWave = wave.Key;
                }
            }

            // return the dominant wave
            return dominantWave;
        }

        private Dictionary<string, float> GetWaveMagnitudes(float[] samples)
        {
            int N = samples.Length; // number of samples
            Complex[] fftInput = new Complex[N];

            // prepare data for fft
            for (int i = 0; i < N; i++)
            {
                fftInput[i] = new Complex(samples[i], 0);
            }

            // apply fft
            Fourier.Forward(fftInput, FourierOptions.NoScaling);

            // get magnitudes
            float[] magnitudes = fftInput.Select(c => (float)c.Magnitude).ToArray();

            // frequency step size
            float frequencyResolution = (float)samplingRate / N;

            // store energy per wave band
            Dictionary<string, float> waveMagnitudes = new Dictionary<string, float>();

            // initialize sums
            float deltaSum = 0f;
            float thetaSum = 0f;
            float alphaSum = 0f;
            float betaSum = 0f;
            float gammaSum = 0f;

            // loop through magnitudes
            for (int i = 0; i < magnitudes.Length; i++)
            {
                // Calculamos la frecuencia de este índice
                float frequency = i * frequencyResolution;

                // Sumamos la energía según el rango de frecuencia que corresponde a cada tipo de onda
                if (frequency < 4)
                    deltaSum += magnitudes[i];
                else if (frequency >= 4 && frequency < 8)
                    thetaSum += magnitudes[i];
                else if (frequency >= 8 && frequency < 12)
                    alphaSum += magnitudes[i];
                else if (frequency >= 12 && frequency < 30)
                    betaSum += magnitudes[i];
                else if (frequency >= 30 && frequency < 45)
                    gammaSum += magnitudes[i];
            }

            // store results
            waveMagnitudes["Delta"] = deltaSum;
            waveMagnitudes["Theta"] = thetaSum;
            waveMagnitudes["Alpha"] = alphaSum;
            waveMagnitudes["Beta"] = betaSum;
            waveMagnitudes["Gamma"] = gammaSum;

            // normalize to 1
            float totalEnergy = deltaSum + thetaSum + alphaSum + betaSum + gammaSum;

            if (totalEnergy > 0)
            {
                waveMagnitudes["Delta"] /= totalEnergy;
                waveMagnitudes["Theta"] /= totalEnergy;
                waveMagnitudes["Alpha"] /= totalEnergy;
                waveMagnitudes["Beta"] /= totalEnergy;
                waveMagnitudes["Gamma"] /= totalEnergy;
            }

            // return energy dictionary
            return waveMagnitudes;
        }
        private void ChangeObjectColor(string waveType)
        {
            if (targetObjectRenderer != null)
            {
                Color color = Color.white;

                switch (waveType)
                {
                    case "Delta":
                        color = Color.red;
                        break;
                    case "Theta":
                        color = Color.blue;
                        break;
                    case "Alpha":
                        color = Color.green;
                        break;
                    case "Beta":
                        color = Color.yellow;
                        break;
                    case "Gamma":
                        color = Color.magenta;
                        break;
                }

                targetObjectRenderer.material.color = color;
            }
        }

        private void ChangeGeneralMood(string waveType)
        {
            switch (waveType)
            {
                case "Delta":
                    generalController.Mood = "neutral";
                    break;
                case "Theta":
                    generalController.Mood = "sad";
                    break;
                case "Alpha":
                    generalController.Mood = "calm";
                    break;
                case "Beta":
                    generalController.Mood = "stressed";
                    break;
                case "Gamma":
                    generalController.Mood = "anxious";
                    break;
            }
        }

        private void CalculateMoodScore(string mood)
        {
            switch (mood)
            {
                case "calm":
                    scoreSystem.PointsEarned = 3;
                    break;
                case "neutral":
                    scoreSystem.PointsEarned = 2;
                    break;
                case "sad":
                    scoreSystem.PointsEarned = 1;
                    break;
                case "stressed":
                    scoreSystem.PointsEarned = 0;
                    break;
                case "anxious":
                    scoreSystem.PointsEarned = 0;
                    break;
            }

            if (scoreSystem.PointsEarned > -1)
            {
                scoreSystem.Score += scoreSystem.PointsEarned;
                scoreSystem.pointsEarnedText.enabled = true;
                timeSinceLastScore = 0f;
            }
            else
            {
                scoreSystem.pointsEarnedText.enabled = false;
            }
        }

        public void showScore(double actualPoints)
        {
            scoreText.text = actualPoints.ToString();
        }
        public void showPoints(double actualPoints)
        {
            pointsText.text = "+" + actualPoints.ToString();
        }

        
    }
}
