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
        public string lastWaveType = ""; // Last registered wave
        private bool waveChanged = false; // Indicates if there was a wave change
        private float timeSinceLastScore = 0f; // time since last score
        private float scoreInterval = 10f; // interval to add points
        private int points = 0;

        private Dictionary<int, Queue<float>> channelBuffers = new Dictionary<int, Queue<float>>();
        private Dictionary<int, float> dominantFrequencies = new Dictionary<int, float>();

        float averageFrequency = 0f;

        public float AverageFrequency {  get { return averageFrequency; } set { averageFrequency = value; } }

        public void StartMuseController()
        {
            for (int channel = 0; channel < numChannels; channel++)
            {
                channelBuffers[channel] = new Queue<float>();
            }
        }
        protected override void Process(float[] newSample, double timeStamp)
        {
            for(int channel = 0; channel < 5; channel++)
            {
                channelBuffers[channel].Enqueue(newSample[channel]);
                if(channelBuffers[channel].Count > bufferSize)
                {
                    channelBuffers[channel].Dequeue();
                }
            }

            bool allBuffersFull = true;

            for(int channel = 0; channel < numChannels; channel++)
            {
                if (channelBuffers[channel].Count != bufferSize)
                {
                    allBuffersFull = false;
                    break;
                }

            }
            // Solo analizar si todos los buffers están llenos
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

                // Creamos un diccionario para contar cuántas veces aparece cada onda
                Dictionary<string, int> waveCounts = new Dictionary<string, int>();

                // Contamos las apariciones
                foreach (string wave in waveTypes)
                {
                    if (!waveCounts.ContainsKey(wave))
                        waveCounts[wave] = 1;
                    else
                        waveCounts[wave]++;
                }

                // Ahora buscamos cuál es la onda que más veces se repite
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

                // Ahora trabajas con finalWave como la onda dominante para todo el conjunto
                waveType = finalWave;

                // Agregar la onda detectada al buffer para comprobar consistencia
                waveBuffer.Enqueue(waveType);
                if (waveBuffer.Count > 5)
                    waveBuffer.Dequeue();

                // Verificar si las últimas ondas son iguales
                bool consistent = true;

                foreach(var wave in waveBuffer)
                {
                    if(wave != waveType)
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
                //showState(waveType, dominantFrequency);
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
            // Llamamos a otra función que calcula cuánta energía tiene cada tipo de onda cerebral
            var waveMagnitudes = GetWaveMagnitudes(samples);

            // Encontramos cuál onda tiene la energía más alta (la que domina)
            string dominantWave = null;
            float highestEnergy = 0f;

            // Revisamos cada tipo de onda y su energía para encontrar la más grande
            foreach (var wave in waveMagnitudes)
            {
                if (wave.Value > highestEnergy)
                {
                    highestEnergy = wave.Value;  // Guardamos la energía más alta encontrada
                    dominantWave = wave.Key;      // Guardamos el nombre de la onda con más energía
                }
            }

            // Devolvemos el nombre de la onda dominante
            return dominantWave;
        }

        private Dictionary<string, float> GetWaveMagnitudes(float[] samples)
        {
            int N = samples.Length; // Número de muestras que tenemos
            Complex[] fftInput = new Complex[N];

            // Preparamos los datos para hacer la Transformada Rápida de Fourier (FFT)
            for (int i = 0; i < N; i++)
            {
                // Convertimos cada muestra en un número complejo (parte real es la muestra, parte imaginaria es 0)
                fftInput[i] = new Complex(samples[i], 0);
            }

            // Aplicamos la FFT para obtener las frecuencias y su intensidad
            Fourier.Forward(fftInput, FourierOptions.NoScaling);

            // Sacamos la magnitud (intensidad) de cada frecuencia calculada
            float[] magnitudes = fftInput.Select(c => (float)c.Magnitude).ToArray();

            // Calculamos cuánto abarca cada "paso" en frecuencias
            float frequencyResolution = (float)samplingRate / N;

            // Aquí vamos a guardar cuánta energía hay en cada banda de ondas cerebrales
            Dictionary<string, float> waveMagnitudes = new Dictionary<string, float>();

            // Inicializamos los contadores para cada banda
            float deltaSum = 0f;
            float thetaSum = 0f;
            float alphaSum = 0f;
            float betaSum = 0f;
            float gammaSum = 0f;

            // Recorremos todas las frecuencias calculadas
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

            // Guardamos los resultados en el diccionario
            waveMagnitudes["Delta"] = deltaSum;
            waveMagnitudes["Theta"] = thetaSum;
            waveMagnitudes["Alpha"] = alphaSum;
            waveMagnitudes["Beta"] = betaSum;
            waveMagnitudes["Gamma"] = gammaSum;

            // Normalizamos para que la suma de todas las energías sea 1 (100%)
            float totalEnergy = deltaSum + thetaSum + alphaSum + betaSum + gammaSum;

            if (totalEnergy > 0)
            {
                waveMagnitudes["Delta"] /= totalEnergy;
                waveMagnitudes["Theta"] /= totalEnergy;
                waveMagnitudes["Alpha"] /= totalEnergy;
                waveMagnitudes["Beta"] /= totalEnergy;
                waveMagnitudes["Gamma"] /= totalEnergy;
            }

            // Devolvemos el diccionario con la energía normalizada de cada onda cerebral
            return waveMagnitudes;
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
