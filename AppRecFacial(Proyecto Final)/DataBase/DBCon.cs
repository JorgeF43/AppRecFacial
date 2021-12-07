using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reconocimiento_facial.DataBase
{
    public class DBCon
    {
        private OleDbConnection conn;
        public string[] Name;
        private byte[] face;
        public List<byte[]> Face = new List<byte[]>();
        public int TotalUser;

        public DBCon()
        {
            conn = new OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = Desaparecidos.mdb");
            conn.Open();
        }

        public bool GuardarImagen(string Name, string Phone, byte[] abImagen)
        {
            conn.Open();
            OleDbCommand comm = new OleDbCommand("INSERT INTO info (nombre,contacto,caras) VALUES ('" + Name + "','" + Phone + "',?)", conn);           
            OleDbParameter parImagen = new OleDbParameter("@caras", OleDbType.VarBinary, abImagen.Length);
            parImagen.Value = abImagen;
            comm.Parameters.Add(parImagen);            
            int iResultado = comm.ExecuteNonQuery();
            conn.Close();
            return Convert.ToBoolean(iResultado);
        }

        public DataTable ObtenerBytesImagen()
        {
            string sql = "SELECT id,nombre,contacto,caras FROM info";
            OleDbDataAdapter adaptador = new OleDbDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);
            int cont = dt.Rows.Count;
            Name = new string[cont];

            for (int i = 0; i < cont; i++)
            {
                Name[i] = dt.Rows[i]["nombre"].ToString();
                face = (byte[])dt.Rows[i]["caras"];
                Face.Add(face);
            }
            TotalUser = dt.Rows.Count;
            conn.Close();
            return dt;
        }

        public void ConvertImgToBinary(string Name, string Phone, Image Img)
        {
            Bitmap bmp = new Bitmap(Img);
            MemoryStream MyStream = new MemoryStream();
            bmp.Save(MyStream, System.Drawing.Imaging.ImageFormat.Bmp);

            byte[] abImagen = MyStream.ToArray();
            GuardarImagen(Name, Phone, abImagen);
        }

        public Image ConvertByteToImg( int con)
        {
            Image FetImg;
            byte[] img = Face[con];
            MemoryStream ms = new MemoryStream(img);
            FetImg = Image.FromStream(ms);
            ms.Close();
            return FetImg;

        }
    }
}
