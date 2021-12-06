using System;
//This is the class in which I take care of the neural network part.
//This does not derive from a MonoBehaviour and is a stand - alone solution for the AI I am using in the game.
public class NeuralNetwork
{
    Layer[] m_Layers;
    public NeuralNetwork(int[] structureOfTheNetwork)
    {
        //Create the layers excluding the input layer. This variable will contain all the layers excluding the input layer. 
        m_Layers = new Layer[structureOfTheNetwork.Length - 1];

        for(int i = 0; i < m_Layers.Length; i++)
        {
            m_Layers[i] = new Layer(structureOfTheNetwork[i], structureOfTheNetwork[i + 1]);
        }
    }
    public float[] FeedForward(float[] inputs) //The feed forward function that is exposed to outside of the class. This function is called when the network is wanted to be trained.
    {
        m_Layers[0].FeedForwardInternal(inputs);
        for(int i = 1; i < m_Layers.Length; i++)
        {
            m_Layers[i].FeedForwardInternal(m_Layers[i - 1].outputs);
        }
        return m_Layers[m_Layers.Length - 1].outputs;
    }
    public void BackProp(float[] expected)
    {
        for (int i = m_Layers.Length - 1; i >= 0; i--)
        {
            if(i == m_Layers.Length - 1)
            {
                m_Layers[i].BackPropOutput(expected);
            }
            else
            {
                m_Layers[i].BackPropHidden(m_Layers[i + 1].gamma, m_Layers[i + 1].weights);
            }
        }

        for(int i = 0; i < m_Layers.Length; i++)
        {
            m_Layers[i].UpdateWeights();
        }
    }
    public class Layer
    {
        int numberOfInputs; //neurons in the previous layer
        int numberOfOutputs; // neurons in the current layer

        public float[] outputs;
        public float[] inputs;
        public float[,] weights;
        public float[,] weightsDelta;
        public float[] gamma;
        public float[] error;
        public static Random random = new Random();

        public Layer(int numberOfInputs, int numberOfOutputs)
        {
            this.numberOfInputs = numberOfInputs;
            this.numberOfOutputs = numberOfOutputs;

            //Initialize the data structures that will form the neural network.
            outputs = new float[numberOfOutputs];
            inputs = new float[numberOfInputs];
            weights = new float[numberOfOutputs, numberOfInputs];
            weightsDelta = new float[numberOfOutputs, numberOfInputs];
            gamma = new float[numberOfOutputs];
            error = new float[numberOfOutputs];

            InitializeWeights();
        }

        private void InitializeWeights() //This function is used to initialize the weights with random balues between -0.5 and 0.5.
        {
            for(int i = 0; i < numberOfOutputs; i++)
            {
                for(int j = 0; j < numberOfInputs; j++)
                {
                    weights[i, j] = (float)random.NextDouble() - 0.5f;
                }
            }
        }
        public float[] FeedForwardInternal(float[] inputs)
        {
            this.inputs = inputs;

            for(int i = 0; i < numberOfOutputs; i++)
            {
                outputs[i] = 0;
                for(int j = 0; j < numberOfInputs; j++)
                {
                    outputs[i] += inputs[j] * weights[i, j]; //Standart input * weight multiplication is done here for each input on a layer. Note: There is no bias used in the calculation.
                }

                outputs[i] = (float)Math.Tanh(outputs[i]);
            }
            return outputs;
        }

        public float TanHDer(float value) //Our activation function.
        {
            return 1 - (value * value);
        }

        public void BackPropOutput(float[] expected)
        {
            for(int i = 0; i < numberOfOutputs; i++)
                error[i] = outputs[i] - expected[i];

            for(int i = 0; i < numberOfOutputs; i++)
                gamma[i] = error[i] * TanHDer(outputs[i]);

            for(int i = 0; i < numberOfOutputs; i++)
            {
                for(int j = 0; j < numberOfInputs; j++)
                {
                    weightsDelta[i, j] = gamma[i] * inputs[j];
                }
            }
        }
        public void BackPropHidden(float[] gammaForward, float[,] weightsForward)
        {
            for(int i = 0; i < numberOfOutputs; i++)
            {
                gamma[i] = 0;

                for(int j = 0; j < gammaForward.Length; j++)
                {
                    gamma[i] += gammaForward[j] * weightsForward[j, i];
                }
                gamma[i] *= TanHDer(outputs[i]);
            }
            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weightsDelta[i, j] = gamma[i] * inputs[j];
                }
            }
        }
        public void UpdateWeights()
        {
            for(int i = 0; i < numberOfOutputs; i++)
            {
                for(int j = 0; j < numberOfInputs; j++)
                {
                    weights[i, j] -= weightsDelta[i, j] * 0.00333f; //TODO: Learning rate is hard coded.
                }
            }
        }
    }
}
