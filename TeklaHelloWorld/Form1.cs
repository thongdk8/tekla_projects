using System;
using System.Windows.Forms;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;

using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;
using TSG = Tekla.Structures.Geometry3d;

namespace TeklaHelloWorld
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create model instace and check connection status
            Model model = new Model();
            if (!model.GetConnectionStatus())
            {
                MessageBox.Show("Tekla structures not connected");
                return;
            }

            //Get model info and send "hello world" message to message box
            ModelInfo modelInfo = model.GetInfo();
            string name = modelInfo.ModelName;
            MessageBox.Show(string.Format("Hello world! your current model is named: {0}", name));

            //Send a hello world to Tekla command prompt
            //Operation.DisplayPrompt(string.Format("Hello world!your current model is named: { 0}", name));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Model myModel = new Model();
            if (myModel.GetConnectionStatus())
            {
                Beam myBeam = new Beam(new TSG.Point(1000, 1000, 1000), new TSG.Point(6000, 6000, 1000));
                myBeam.Material.MaterialString = "S235JR";
                myBeam.Profile.ProfileString = "HEA400";

                myBeam.Insert();
                myModel.CommitChanges();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ModelObjectEnumerator.AutoFetch = true;
            var modelObjectEnumerator =
                new Model().GetModelObjectSelector().GetAllObjects();

            //var modelObjects = modelObjectEnumerator.ToList();
            var modelObjects = ToIEnumerable(modelObjectEnumerator);

            var beams = modelObjects.OfType<Beam>().ToList();

            var beamsReport = beams.Select(b => new
            {
                b.Name,
                b.Profile.ProfileString,
                b.Class,
                b.Finish,
                b.Material.MaterialString,
                b.Profile
            })
                .ToList();

            var modelName = new Model().GetInfo().ModelName.Split('.')[0];

            var filePath = $"D:\\{modelName}_BeamReport.csv";
            using (var writer = new CsvWriter(new StreamWriter(filePath)))
            {
                writer.WriteRecords(beamsReport);
            }
        }


        public static IEnumerable<ModelObject> ToIEnumerable(ModelObjectEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }
}
