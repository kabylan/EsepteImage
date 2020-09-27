using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using EsepteImage.MachineLearningNoticeProperty.Model;
using Microsoft.ML;

namespace EsepteImage
{
    public partial class Form1 : Form
    {
        string selectedFolderPath = "";

        string[] filesPaths = { "" };

        List<ImageType> ImageTypes;

        int[] tabsImages = new int[8];


        private static Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictionEngine = null;
        public static string MLNetModelPath = Path.GetFullPath("MachineLearningNoticeProperty/ntsprmdl.zip");

        public Form1()
        {
            MLContext mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
            PredictionEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(predEngine);

            InitializeComponent();
        }

        public static ModelOutput Predict(ModelInput input)
        {
            ModelOutput result = PredictionEngine.Value.Predict(input);
            return result;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // get path
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                selectedFolderPath = folderBrowserDialog1.SelectedPath;

                InitializeArrayWithFilesPaths();
            }

        }

        private void InitializeArrayWithFilesPaths()
        {
            filesPaths = Directory.GetFiles(selectedFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".jpeg")).ToArray();

            label4.Text = filesPaths.Length + "";
        }

        private string Recognize(string imagePath)
        {

            ModelInput sampleData = new ModelInput()
            {
                ImageSource = imagePath,
            };

            var predictionResult = Predict(sampleData);

            

            return predictionResult.Prediction;
        }

        private string GetTypeRu(string Prediction)
        {
            string typeRU = "";
            switch (Prediction)
            {
                case "2d_plan":
                    typeRU = "2d планировака";
                    break;
                case "2d_plan_photo":
                    typeRU = "документ";
                    break;
                case "3d_plan":
                    typeRU = "3d планировака";
                    break;
                case "artwork":
                    typeRU = "логотип";
                    break;
                case "desk_bred_property":
                    typeRU = "спам АН";
                    break;
                case "desk_bred_text_and_photo":
                    typeRU = "спам";
                    break;
                case "document":
                    typeRU = "документ";
                    break;
                case "map":
                    typeRU = "карта";
                    break;
                case "poster":
                    typeRU = "спам";
                    break;
                case "real":
                    typeRU = "реальное";
                    break;
                default:
                    typeRU = "-";
                    break;
            }
            return typeRU;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            progressBar1.Value = 1;

            int x = 0;
            int y = 0;
            int i = 0;
            int n = 0;

            ImageTypes = new List<ImageType>();

            foreach (string filePath in filesPaths)
            {
                string result = Recognize(filePath);

                label6.Text = GetTypeRu(result);

                n++;
                if (n < 100)
                {
                    i++;

                    var pctBox = new PictureBox();
                    pctBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pctBox.Image = Image.FromFile(filePath);
                    pctBox.Location = new Point(x, y);

                    pctBox.Width = 250;
                    pctBox.Height = 220;
                    x += pctBox.Width + 2;
                    if (i > 5)
                    {
                        i = 0;
                        y += pctBox.Height + 2;
                        x = 0;
                    }
                    ImageTypes.Add(new ImageType { ImagePath = filePath, Prediction = result });
                    AddToTab(pctBox, result);
                }
            }

            tabControl2.Visible = true;
            label4.Visible = true;
        }

        private void AddToTab(PictureBox pctBox, string result)
        {

            switch (result)
            {
                case "2d_plan":
                    tabsImages[0]++;
                    tabPage3.Controls.Add(pctBox);
                    break;
                case "2d_plan_photo":
                    tabsImages[1]++;
                    tabPage4.Controls.Add(pctBox);
                    break;
                case "3d_plan":
                    tabsImages[3]++;
                    tabPage6.Controls.Add(pctBox);
                    break;
                case "artwork":
                    tabsImages[4]++;
                    tabPage7.Controls.Add(pctBox);
                    break;
                case "desk_bred_property":
                    tabsImages[5]++;
                    tabPage8.Controls.Add(pctBox);
                    break;
                case "desk_bred_text_and_photo":
                    tabsImages[6]++;
                    tabPage9.Controls.Add(pctBox);
                    break;
                case "document":
                    tabsImages[1]++;
                    tabPage4.Controls.Add(pctBox);
                    break;
                case "map":
                    tabsImages[7]++;
                    tabPage10.Controls.Add(pctBox);
                    break;
                case "poster":
                    tabsImages[6]++;
                    tabPage9.Controls.Add(pctBox);
                    break;
                case "real":
                    tabsImages[2]++;
                    tabPage5.Controls.Add(pctBox);
                    break;
                default:
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }


    public class ImageType
    {
        public string Prediction { get; set; }
        public string ImagePath { get; set; }
    }
}

/*
2d планировака
документ
3d планировака
логотип
спам АН
спам
карта
реальное
 */