using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace RUAP_Projekt
{
    public partial class Form1 : Form
    {
        Random random = new Random();

		public Form1()
        {
            InitializeComponent();
        }

        private void buttonRandom_Click(object sender, EventArgs e)
        {
            textBoxPh_Value.Text = random.Next(0, 14).ToString();
            textBoxHardness_Value.Text = random.Next(80, 300).ToString();
            textBoxSolids_Value.Text = random.Next(8000, 50000).ToString();
            textBoxChloramines_Value.Text = random.Next(3, 11).ToString();
            textBoxSulfate_Value.Text = random.Next(250, 500).ToString();
            textBoxConductivity_Value.Text = random.Next(200, 700).ToString();
            textBoxOrganicCarbon_Value.Text = random.Next(5, 25).ToString();
            textBoxTrihalomethanes_Value.Text = random.Next(5, 110).ToString();
            textBoxTurbidity_Value.Text = random.Next(1, 10).ToString();
        }

        private async void buttonGetResults_Click(object sender, EventArgs e)
        {
            await InvokeRequestResponseService(textBoxPh_Value.Text, textBoxHardness_Value.Text, textBoxSolids_Value.Text, textBoxChloramines_Value.Text, 
                textBoxSulfate_Value.Text,textBoxConductivity_Value.Text, textBoxOrganicCarbon_Value.Text, textBoxTrihalomethanes_Value.Text, textBoxTurbidity_Value.Text,
                richTextBox1);
        }

        private async Task InvokeRequestResponseService(string ph, string hardness, string solids, string chloramines, string sulfate, string conductivity,
            string organicCarbon, string trihalomethanes, string turbidity, RichTextBox richTextBox)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
                                     {
                                         "ph", ph
                                     },
                                     {
                                         "Hardness", hardness
                                     },
                                     {
                                         "Solids", solids
                                     },
                                     {
                                         "Chloramines", chloramines
                                     },
                                     {
                                         "Sulfate", sulfate
                                     },
                                     {
                                         "Conductivity", conductivity
                                     },
                                     {
                                         "Organic_carbon", organicCarbon
                                     },
                                     {
                                         "Trihalomethanes", trihalomethanes
                                     },
                                     {
                                         "Turbidity", turbidity
                                     },
                                     {
                                         "Potability", "0"
                                     },
                                }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                const string apiKey = "qiEoLzmrkRO74eBx6oLxIrt9jAkJbEqm7chvhcddgQvJUKm2SH6/aLVHnH8CSG3K+J+r6gDGYd+G+AMCBzq3MA=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/290841c73b2340499f63db5000f97aca/services/ad91691ec5894e99a68c8d7b2ac35369/execute?api-version=2.0&format=swagger");


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    richTextBox.Clear();
                    string result = await response.Content.ReadAsStringAsync();
                    var formattedResult = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(result), Formatting.Indented);
                    Console.WriteLine("Result: {0}", formattedResult);

                    richTextBox.AppendText("Result:" + Environment.NewLine);
                    richTextBox.AppendText(formattedResult);

                    if (formattedResult.Contains("\"Scored Labels\": \"1\""))
					{
                        Console.WriteLine("Water with entered properties is drinkable!");
                        lblresult.Text = String.Format("Water with entered properties \n is drinkable!");
                        lblresult.ForeColor = Color.Green;
                        this.pictureBox1.Visible = true;
                        this.pictureBox2.Visible = false;

                    }
					else
					{
                        Console.WriteLine("Water with entered properties is not drinkable!");
                        lblresult.Text = String.Format("Water with entered properties \n is not drinkable!");
                        lblresult.ForeColor = Color.Red;
                        //richTextBox.AppendText("\n\n Water with entered properties is not drinkable!");
                        this.pictureBox2.Visible = true;
                        this.pictureBox1.Visible = false;
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));
                    Console.WriteLine(response.Headers.ToString());
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }
        }
    }
}
