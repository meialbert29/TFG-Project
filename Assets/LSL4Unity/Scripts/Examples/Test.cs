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
        public ScoreSystem scoreSystem;

        public string lastSample = String.Empty;
        public int samplingRate = 256; // Frecuencia de muestreo
        private int bufferSize = 256; // Número de muestras para FFT
        private Queue<float> eegBuffer = new Queue<float>(); // Buffer de muestras EEG
        public Renderer targetObjectRenderer; // Objeto que cambiará de color

        public string waveType;
        public float dominantFrequency;

        public Text emotionalStateText;
        public Text scoreText;
        public Text pointsText;

        private double actualScore = 0;

        private Queue<string> waveBuffer = new Queue<string>(); // Almacena las últimas 5 ondas
        public string lastWaveType = ""; // Última onda registrada
        private bool waveChanged = false; // Indica si ha habido un cambio de onda
        private float timeSinceLastScore = 0f; // Tiempo desde la última puntuación
        private float scoreInterval = 10f; // Intervalo de tiempo para sumar puntos
        private int points = 0;

        //public WavesReader wavesReader;

        protected override void Process(float[] newSample, double timeStamp)
        {
            eegBuffer.Enqueue(newSample[0]); // Usamos el primer canal
            if (eegBuffer.Count > bufferSize)
                eegBuffer.Dequeue(); // Mantener el buffer con tamaño adecuado

            if (eegBuffer.Count == bufferSize)
            {
                dominantFrequency = AnalyzeEEG(eegBuffer.ToArray());
                waveType = ClassifyBrainWave(dominantFrequency);

                // Agregar la onda detectada al buffer
                waveBuffer.Enqueue(waveType);
                if (waveBuffer.Count > 5)
                    waveBuffer.Dequeue(); // Mantener solo las últimas 5 ondas

                // Evaluar consistencia de ondas en las últimas 5 muestras
                bool consistent = waveBuffer.All(w => w == waveType);

                if (consistent)
                {

                    if (waveType != lastWaveType)
                    {
                        scoreSystem.pointsEarnedText.enabled = false;
                        if (waveChanged)
                        {
                            points = 2;
                            actualScore += points; // Si cambia a una nueva onda tras otra, sumar menos

                            scoreSystem.PointsEarned = 2;
                            scoreSystem.Score += points;
                        }
                        else
                        {
                            scoreSystem.pointsEarnedText.enabled = true;
                            scoreSystem.PointsEarned = 1;
                            scoreSystem.Score += points; // Si cambia de onda y se mantiene, sumar más puntos

                        }
                        waveChanged = true;
                        timeSinceLastScore = 0f; // Reiniciar el contador de tiempo
                    }
                    else
                    {
                        timeSinceLastScore += Time.deltaTime;
                        if (timeSinceLastScore >= scoreInterval)
                        {
                            scoreSystem.pointsEarnedText.enabled = true;
                            scoreSystem.PointsEarned = 20; // add points if the wave stays the same 10 secs
                            scoreSystem.Score += scoreSystem.PointsEarned;
                            timeSinceLastScore = 0f; // Reiniciar el temporizador
                        }
                    }
                }
                else
                {
                    scoreSystem.pointsEarnedText.enabled = false;
                    waveChanged = false; // Si la onda no es consistente, esperar un cambio estable
                    timeSinceLastScore = 0f; // Reiniciar el contador si hay inestabilidad
                }

                lastWaveType = waveType; // Actualizar última onda detectada

                ChangeObjectColor(waveType);
                showState(waveType, dominantFrequency);
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

        private string ClassifyBrainWave(float frequency)
        {
            if (frequency < 4) return "Delta";
            if (frequency < 8) return "Theta";
            if (frequency < 14) return "Alpha";
            if (frequency < 30) return "Beta";
            return "Gamma";
        }

        private Dictionary<string, float> GetWaveMagnitudes(float[] samples)
        {
            int N = samples.Length;
            Complex[] fftInput = new Complex[N];

            for (int i = 0; i < N; i++)
                fftInput[i] = new Complex(samples[i], 0);

            Fourier.Forward(fftInput, FourierOptions.NoScaling);

            float[] magnitudes = fftInput.Select(c => (float)c.Magnitude).ToArray();
            float frequencyResolution = (float)samplingRate / N;

            Dictionary<string, float> waveMagnitudes = new Dictionary<string, float>
            {
                { "Delta", magnitudes.Where((_, i) => i * frequencyResolution < 4).Sum() },
                { "Theta", magnitudes.Where((_, i) => i * frequencyResolution >= 4 && i * frequencyResolution < 8).Sum() },
                { "Alpha", magnitudes.Where((_, i) => i * frequencyResolution >= 8 && i * frequencyResolution < 12).Sum() },
                { "Beta", magnitudes.Where((_, i) => i * frequencyResolution >= 12 && i * frequencyResolution < 30).Sum() },
                { "Gamma", magnitudes.Where((_, i) => i * frequencyResolution >= 30).Sum() }
            };

            return waveMagnitudes;
        }

        private double Normalize(double value, double min, double max)
        {
            return (value - min) / (max - min);
        }

        private void CalculateConcentrationScore(string wave)
        {
            switch (wave)
            {
                case "Delta":
                    actualScore += 10;
                    break;
                case "Theta":
                    actualScore += 5;
                    break;
                case "Alpha":
                    actualScore += 3;
                    break;
                case "Beta":
                    actualScore += 2;
                    break;
                case "Gamma":
                    actualScore += 1;
                    break;
            }
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

        public void showState(string wave, float frequency)
        {
            emotionalStateText.text = "Wave " + wave + "\nFrequency " + frequency;
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
