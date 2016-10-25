using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperNova
{
    public partial class Form1 : Form
    {

        private byte[] savedata = null;

        class Item
        {
            //i want to find more about this value
            public Int32 UID { get; set; }

            //i don't know exactly what these are but
            //there is 0xC from the above number until the item amount



                //XX 00 YY 00 00 00 ZZ 00 00 00 00 00 ?? 32 06



//XX : Placeholder/Item Type = 01 : Weapon, 05 : Shield. (Only ones I found so far)

            public Int16 ItemType { get; set; }
            public Int32 ItemCategory { get; set; }
            public Int16 ItemID { get; set; }
            //flag the item green or not
            public Int16 New { get; set; }



            //yes it is 16bit :)
            public UInt16 Quantity { get; set; }

            //affix/effect whatever, 16bit value
            //maybe dependdant on the next 16bit being equal to 6?
            public UInt16 Affix { get; set; }

            //i don't know
            public Int32 unknown1 { get; set; }
            public Int32 unknown2 { get; set; }
            public Int32 unknown3 { get; set; }
            public Int32 unknown4 { get; set; }
            public Int32 unknown5 { get; set; }
            public Int32 unknown6 { get; set; }
            public Int16 unknown7 { get; set; }

        }

        List<Item> inventoryList = new List<Item>();



        public Form1()
        {
            InitializeComponent();


            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;

//            dataGridView1.Columns.Add("UIDCol", "UID");
//            dataGridView1.Columns.Add("typeCol", "Type");
//            dataGridView1.Columns.Add("categoryCol", "Category");
//            dataGridView1.Columns.Add("IDCol", "ID");
//            dataGridView1.Columns.Add("quantityCol", "Quantity");
//            dataGridView1.Columns.Add("affixCol", "Affix");






        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProcessSave();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExportSave();
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }


        public void ProcessSave()
        {
            Stream myStream = null;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Save files (*.bin)|*.bin|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // Insert code to read the stream here.

                            int nameStart = 0x8;
                            int namelength = 0x14;

                            BinaryReader breader = new BinaryReader(myStream);

                            savedata = breader.ReadBytes((int)myStream.Length);

                            long length = myStream.Length;

                            breader.BaseStream.Position = nameStart;
                            byte[] itemSection = breader.ReadBytes(namelength);

                            var mystring = Encoding.Unicode.GetString(itemSection);
                            textBox1.Text = mystring.ToString();

                            //game time
                            breader.BaseStream.Position = 0x20;
                            int playseconds = breader.ReadInt32();

                            TimeSpan t = TimeSpan.FromSeconds(playseconds);

                            hoursUD.Text = Math.Floor(t.TotalHours).ToString();
                            minutesUD.Text = t.Minutes.ToString();
                            secondsUD.Text = t.Seconds.ToString();



                            //hunter level
                            breader.BaseStream.Position = 0x3b448;

                            hunterlevelUD.Text = breader.ReadInt32().ToString();
                            hunterexpUD.Text = breader.ReadInt32().ToString();

                            rangerlevelUD.Text = breader.ReadInt32().ToString();
                            rangerexpUD.Text = breader.ReadInt32().ToString();

                            forcelevelUD.Text = breader.ReadInt32().ToString();
                            forceexpUD.Text = breader.ReadInt32().ToString();

                            busterlevelUD.Text = breader.ReadInt32().ToString();
                            busterexpUD.Text = breader.ReadInt32().ToString();


                            //gran energy
                            breader.BaseStream.Position = 0x4442c;
                            granenergyUD.Text = breader.ReadInt32().ToString();





                            breader.BaseStream.Position = 0x30;
                            inventoryList.Clear(); //reset since if we open more saves, it would just appened
                            dataGridView1.Rows.Clear();

                            for (int i = 0; i < 199; i++) //0 to 99, 100.
                            {
                                Item tempItem = new Item();

                                tempItem.UID = breader.ReadInt32();
                                tempItem.ItemType = breader.ReadInt16();
                                tempItem.ItemCategory = breader.ReadInt32();
                                tempItem.ItemID = breader.ReadInt16();
                                tempItem.New = breader.ReadInt16();
                                tempItem.Quantity = breader.ReadUInt16();
                                tempItem.Affix = breader.ReadUInt16();
                                tempItem.unknown1 = breader.ReadInt32();
                                tempItem.unknown2 = breader.ReadInt32();
                                tempItem.unknown3 = breader.ReadInt32();
                                tempItem.unknown4 = breader.ReadInt32();
                                tempItem.unknown5 = breader.ReadInt32();
                                tempItem.unknown6 = breader.ReadInt32();
                                tempItem.unknown7 = breader.ReadInt16();

                                inventoryList.Add(tempItem);

//                                DataGridViewRow row = new DataGridViewRow();
//                                row.CreateCells(dataGridView1);
//
//                                row.Cells[0].Value = tempItem.UID;
//                                row.Cells[1].Value = tempItem.ItemType;
//                                row.Cells[2].Value = tempItem.ItemCategory;
//                                row.Cells[3].Value = tempItem.ItemID;
//                                row.Cells[4].Value = tempItem.Quantity;
//                                row.Cells[5].Value = tempItem.Affix;
//            
//                                dataGridView1.Rows.Add(row);



                            }

                            //create data sources
                            var itemsdatasource = new BindingSource();
                            itemsdatasource.DataSource = inventoryList;
                            dataGridView1.DataSource = itemsdatasource;
                            dataGridView1.AutoResizeColumns();



                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        public void ExportSave()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Save files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;


            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Stream outstream = saveFileDialog1.OpenFile();

                    using (outstream)
                        {
                            //copy input save data to output stream
                            //myStream.CopyTo(outstream);

                            outstream.Write(savedata,0,savedata.Length);
                            BinaryWriter bwriter = new BinaryWriter(outstream);


                            int nameStart = 0x8;
//                            int namelength = 0x14; //not used for writing...


                            bwriter.BaseStream.Position = nameStart;
                            byte[] namebytes = Encoding.Unicode.GetBytes(textBox1.Text);
                            bwriter.Write(namebytes);

                            

                            //game time
                            bwriter.BaseStream.Position = 0x20;
                            Int32 playseconds = 0;
                            playseconds += (int)secondsUD.Value;
                            playseconds += ( 60 * (int)minutesUD.Value);
                            playseconds += ( 60 * 60 * (int)hoursUD.Value);
                            bwriter.Write((Int32)playseconds);

                     



                            //hunter level
                            bwriter.BaseStream.Position = 0x3b448;

                            bwriter.Write((Int32)hunterlevelUD.Value);
                            bwriter.Write((Int32)hunterexpUD.Value);

                            bwriter.Write((Int32)rangerlevelUD.Value);
                            bwriter.Write((Int32)rangerexpUD.Value);

                            bwriter.Write((Int32)forcelevelUD.Value);
                            bwriter.Write((Int32)forceexpUD.Value);

                            bwriter.Write((Int32)busterlevelUD.Value);
                            bwriter.Write((Int32)busterexpUD.Value);



                            //gran energy
                            bwriter.BaseStream.Position = 0x4442c;
                            bwriter.Write(Convert.ToInt32(granenergyUD.Value));


//                            //scape doll
//                            if (scapedollcheckbox.Checked)
//                            {
//                                bwriter.BaseStream.Position = 0x146;
//                                bwriter.Write(Convert.ToInt16(255));
//                            }



                        //                           outstream.Close();


                        bwriter.BaseStream.Position = 0x30;

                        for (int i = 0; i < 199; i++) //0 to 99, 100.
                        {
                            bwriter.Write((Int32)inventoryList[i].UID);
                            bwriter.Write((Int16)inventoryList[i].ItemType);
                            bwriter.Write((Int32)inventoryList[i].ItemCategory);
                            bwriter.Write((Int16)inventoryList[i].ItemID);
                            bwriter.Write((Int16)inventoryList[i].New);
                            bwriter.Write((UInt16)inventoryList[i].Quantity);
                            bwriter.Write((UInt16)inventoryList[i].Affix);

//                            bwriter.Write((Int32)dataGridView1.Rows[i].Cells[0].Value);
//                            bwriter.Write((Int16)dataGridView1.Rows[i].Cells[1].Value);
//                            bwriter.Write((Int32)dataGridView1.Rows[i].Cells[2].Value);
//                            bwriter.Write((Int32)dataGridView1.Rows[i].Cells[3].Value);
//                            bwriter.Write((Int16)dataGridView1.Rows[i].Cells[4].Value);
//                            bwriter.Write((UInt16)dataGridView1.Rows[i].Cells[5].Value);

                            bwriter.Write((Int32)inventoryList[i].unknown1);
                            bwriter.Write((Int32)inventoryList[i].unknown2);
                            bwriter.Write((Int32)inventoryList[i].unknown3);
                            bwriter.Write((Int32)inventoryList[i].unknown4);
                            bwriter.Write((Int32)inventoryList[i].unknown5);
                            bwriter.Write((Int32)inventoryList[i].unknown6);
                            bwriter.Write((Int16)inventoryList[i].unknown7);

                        }


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

    }
}
