using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FANNCSharp;
using FANNCSharp.Double;
using DataType = System.Double;

namespace BetManagerFannApp
{
    class Program
    {
        static int Main(string[] args)
        {
            bool doTrain = false;
            DataType[] calc_out;
            const uint num_input = 10;
            const uint num_output = 3;
            const uint num_layers = 4;
            const uint num_neurons_hidden = 15;
            const float desired_error = 0;
            uint max_epochs = 20000;
            const uint epochs_between_reports = 10;

            string trainPath = "";
            string netPath = "";
            string testPath = "";
            string csvPath = "";

            if (args.Length == 0)
            {
                System.Console.WriteLine("Please enter a train file path and test file path");
                System.Console.WriteLine("Usage: SportTrain <train-path> <test-path>");
                return 1;
            }
            else
            {
                trainPath = args[0];
                testPath = args[1];

                string dirPath = System.IO.Path.GetDirectoryName(trainPath);
                netPath = System.IO.Path.ChangeExtension(trainPath, "net");
                csvPath = System.IO.Path.ChangeExtension(testPath, "csv");

                // kontrola existence site
                doTrain = !System.IO.File.Exists(netPath);

                if (args.Length > 2)
                {
                    max_epochs = uint.Parse(args[2]);
                }
            }

            Console.WriteLine("Creating network.");

            if (doTrain)
            {
                using (NeuralNet net = new NeuralNet(NetworkType.LAYER, num_layers, num_input, num_neurons_hidden, num_neurons_hidden, num_output))
                using (TrainingData data = new TrainingData(trainPath))
                {
                    net.ActivationFunctionHidden = ActivationFunction.SIGMOID_SYMMETRIC;
                    net.ActivationFunctionOutput = ActivationFunction.SIGMOID_SYMMETRIC;

                    net.TrainStopFunction = StopFunction.STOPFUNC_MSE;
                    // net.BitFailLimit = 0.01F;

                    net.TrainingAlgorithm = TrainingAlgorithm.TRAIN_RPROP;

                    net.InitWeights(data);

                    Console.WriteLine("Training network.");
                    net.TrainOnData(data, max_epochs, epochs_between_reports, desired_error);

                    Console.WriteLine("Testing network");
                    // Keep a copy of the inputs and outputs so that we don't call TrainingData.Input
                    // and TrainingData.Output multiple times causing a copy of all the data on each
                    // call. An alternative is to use the Input/OutputAccessors which are fast with 
                    // repeated calls to get data and can be cast to arrays with the Array property
                    DataType[][] input = data.Input;
                    DataType[][] output = data.Output;
                    for (int i = 0; i < data.TrainDataLength; i++)
                    {
                        calc_out = net.Run(input[i]);
                        Console.WriteLine("XOR test ({0},{1}) -> {2}, should be {3}",
                                            input[i][0], input[i][1], calc_out[0], output[i][0]);
                    }

                    Console.WriteLine("Saving network.\n");

                    net.Save(netPath);
                }
            }

            int ret = 0;
            using (NeuralNet net = new NeuralNet(netPath))
            {
                net.PrintConnections();
                net.PrintParameters();

                Console.WriteLine("Testing network.");

                using (TrainingData data = new TrainingData())
                {
                    if (!data.ReadTrainFromFile(testPath))
                    {
                        Console.WriteLine("Error reading training data --- ABORTING.\n");
                        return -1;
                    }

                    //net.ResetMSE();
                    //net.TestData(data);
                    //Console.WriteLine("MSE {0}", net.MSE);

                    //for (int i = 0; i < data.TrainDataLength; i++)
                    //{
                    //    net.ResetMSE();
                    //    calc_out = net.Test(data.GetTrainInput((uint)i).Array, data.GetTrainOutput((uint)i).Array);
                    //    Console.WriteLine("XOR test ({0}, {1}) -> {2}, should be {3}, difference={4}",
                    //        data.GetTrainInput((uint)i)[0],
                    //        data.GetTrainInput((uint)i)[1],
                    //        calc_out[0],
                    //        data.GetTrainOutput((uint)i)[0],
                    //        calc_out[0] - data.GetTrainOutput((uint)i)[0]);
                    //}

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < data.TrainDataLength; i++)
                    {
                        net.ResetMSE();
                        calc_out = net.Test(data.GetTrainInput((uint)i).Array, data.GetTrainOutput((uint)i).Array);


                        sb.Append(data.GetTrainInput((uint)i)[0]).Append(';')
                            .Append(data.GetTrainInput((uint)i)[1]).Append(';')
                            .Append(data.GetTrainInput((uint)i)[2]).Append(';')
                            .Append(data.GetTrainOutput((uint)i)[0]).Append(';')
                            .Append(data.GetTrainOutput((uint)i)[1]).Append(';')
                            .Append(data.GetTrainOutput((uint)i)[2]).Append(';')
                            .Append(calc_out[0]).Append(';')
                            .Append(calc_out[1]).Append(';')
                            .Append(calc_out[2]).Append(';')
                            .AppendLine();

                        Console.WriteLine("XOR test ({0}, {1}) -> {2}, should be {3}, difference={4}",
                            data.GetTrainInput((uint)i)[0],
                            data.GetTrainInput((uint)i)[1],
                            calc_out[0],
                            data.GetTrainOutput((uint)i)[0],
                            calc_out[0] - data.GetTrainOutput((uint)i)[0]);
                    }

                    System.IO.File.WriteAllText(csvPath, sb.ToString().Replace(',', '.'));
                    Console.WriteLine("Cleaning up.");
                }
            }

            return ret;
        }

        static float FannAbs(float value)
        {
            return (((value) > 0) ? (value) : -(value));
        }
    }
}
