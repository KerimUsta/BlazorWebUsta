using KerimUstaBlazor.Shared;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Tasarým.Shared;
using ustakerimhost.Models;

namespace BlazorWebUsta.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public static string ConnectionString = 
            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.0.150)(PORT = 1521))(CONNECT_DATA=(SERVICE_NAME=TEST)));User Id=ifsapp;Password=ifsapp;";

        [HttpGet]
        [Route("Listele")]
        public List<VeriModel> TestOracleConnection()
        {
            List<VeriModel> veriListesi = new List<VeriModel>();
            using (OracleConnection con = new OracleConnection(ConnectionString))
            {
                OracleCommand cmd = new OracleCommand("select * from Yr_Kerimusta", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    VeriModel model = new VeriModel();
                    model.SIPARIS_NO = dr["SIPARIS_NO"].ToString();
                    model.YAYIN = dr["YAYIN"].ToString();
                    model.REDUKTOR_KOD = dr["REDÜKTÖR_KOD"].ToString();
                    model.REDUKTOR_ACIKLAMA = dr["REDÜKTÖR_ACIKLAMA"].ToString();
                    model.ADET = dr["ADET"].ToString();
                    model.FIYAT = dr["FIYAT"].ToString();
                    model.TOPLAM_SIPARIS_TUTAR = dr["TOPLAM_SIPARIS_TUTAR"].ToString();
                    model.TESLIM_TARIHI = Convert.ToDateTime(dr["TESLIM_TARIHI"]);
                    model.DURUM = dr["DURUM"].ToString();
                    model.GECIKME = dr["GECIKME"].ToString();
                    model.ONAY = dr["ONAY"].ToString();
                    model.SIPARISI_ACAN = dr["SIPARISI_ALAN"].ToString();
                    model.MUSTERI_AD = dr["MÜSTERI_AD"].ToString();
                    veriListesi.Add(model);

                }
                con.Close();

            }
            return veriListesi;
        }

        [HttpGet]
        [Route("ComboDoldur")]
        public List<ComboDoldur> ComboDoldur()
        {
            List<ComboDoldur> model = new List<ComboDoldur>();
            using (OracleConnection con = new OracleConnection(ConnectionString))
            {
                OracleCommand cmd = new OracleCommand("select DISTINCT TRIM(LEADING ' ' FROM x.rowstate) as DURUM from customer_order_line_tab x ORDER BY 1", con);
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ComboDoldur proc = new ComboDoldur();
                    proc.DURUM = dr["DURUM"].ToString();
                    model.Add(proc);
                }
                con.Close();
            }
            return model;
        }

        [HttpPost]
        [Route("Onayla")]
        public Responen Onayla(Onayla onay)
        {
            Responen respons = new Responen();

            if (onay.value == "onayladý")
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    connection.Open();
                    string selectQuery = $"SELECT ONAY FROM Yr_Ustakerim_Proje2_Tab WHERE SIPARIS_NO = '{onay.sip}' AND YAYIN = '{onay.yay}' AND REDUKTOR_KOD = '{onay.reduk}'";
                    using (OracleCommand selectCommand = new OracleCommand(selectQuery, connection))
                    {
                        OracleDataReader reader = selectCommand.ExecuteReader();
                        if (reader.Read())
                        {
                            // Eðer kayýt varsa ONAY durumunu güncelle
                            string updateQuery = "UPDATE Yr_Ustakerim_Proje2_Tab SET ONAY = 'Onaylanmadý', ONAY_TARIH = :onay_tarih WHERE SIPARIS_NO = :sip AND YAYIN = :yay AND REDUKTOR_KOD = :reduk";

                            using (OracleCommand updateCommand = new OracleCommand(updateQuery, connection))
                            {
                                // Parametreleri ekleyin ve deðerlerini ayarlayýn
                                updateCommand.Parameters.Add(new OracleParameter("onay_tarih", DateTime.Now));
                                updateCommand.Parameters.Add(new OracleParameter("sip", onay.sip));
                                updateCommand.Parameters.Add(new OracleParameter("yay", onay.yay));
                                updateCommand.Parameters.Add(new OracleParameter("reduk", onay.reduk));
                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    //dataGridView1.Rows[selectedRowIndex].Cells["ONAY"].Value = "Onaylý";
                                    //MessageBox.Show("Onay durumu güncellendi.");
                                    respons.Response = "Onay Ýþlemi Ýptal Edildi";
                                }
                                else
                                {
                                    respons.Response = "Onaylanmadý";
                                    //MessageBox.Show("Onay durumu güncellenirken hata oluþtu.");
                                }
                            }
                        }
                        else
                        {
                            string insertQuery = "INSERT INTO Yr_Ustakerim_Proje2_Tab (SIPARIS_NO, YAYIN, REDUKTOR_KOD, ONAY, ONAY_TARIH) VALUES (:sip, :yay, :reduk, 'Onaylanmadý', :onay_tarih)";

                            using (OracleCommand insertCommand = new OracleCommand(insertQuery, connection))
                            {
                                // Parametreleri ekleyin ve deðerlerini ayarlayýn
                                insertCommand.Parameters.Add(new OracleParameter("sip", onay.sip));
                                insertCommand.Parameters.Add(new OracleParameter("yay", onay.yay));
                                insertCommand.Parameters.Add(new OracleParameter("reduk", onay.reduk));
                                insertCommand.Parameters.Add(new OracleParameter("onay_tarih", DateTime.Now));
                                int rowsAffected = insertCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    respons.Response = "Onaylandý";
                                }
                                else
                                {
                                    respons.Response = "Onaylanmadý";
                                }
                            }

                        }
                    }
                }
            }
            else if (onay.value == "Onaylanmadý")
            {

                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    connection.Open();
                    string selectQuery = $"SELECT ONAY FROM Yr_Ustakerim_Proje2_Tab WHERE SIPARIS_NO = '{onay.sip}' AND YAYIN = '{onay.yay}' AND REDUKTOR_KOD = '{onay.reduk}'";
                    using (OracleCommand selectCommand = new OracleCommand(selectQuery, connection))
                    {
                        OracleDataReader reader = selectCommand.ExecuteReader();
                        if (reader.Read())
                        {
                            // Eðer kayýt varsa ONAY durumunu güncelle
                            string updateQuery = "UPDATE Yr_Ustakerim_Proje2_Tab SET ONAY = 'Onaylandý', ONAY_TARIH = :onay_tarih WHERE SIPARIS_NO = :sip AND YAYIN = :yay AND REDUKTOR_KOD = :reduk";

                            using (OracleCommand updateCommand = new OracleCommand(updateQuery, connection))
                            {
                                // Parametreleri ekleyin ve deðerlerini ayarlayýn
                                updateCommand.Parameters.Add(new OracleParameter("onay_tarih", DateTime.Now));
                                updateCommand.Parameters.Add(new OracleParameter("sip", onay.sip));
                                updateCommand.Parameters.Add(new OracleParameter("yay", onay.yay));
                                updateCommand.Parameters.Add(new OracleParameter("reduk", onay.reduk));
                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    //dataGridView1.Rows[selectedRowIndex].Cells["ONAY"].Value = "Onaylý";
                                    //MessageBox.Show("Onay durumu güncellendi.");
                                    respons.Response = "Onaylandý";
                                }
                                else
                                {
                                    respons.Response = "Onaylanmadý";
                                    //MessageBox.Show("Onay durumu güncellenirken hata oluþtu.");
                                }
                            }
                        }
                        else
                        {
                            string insertQuery = "INSERT INTO Yr_Ustakerim_Proje2_Tab (SIPARIS_NO, YAYIN, REDUKTOR_KOD, ONAY, ONAY_TARIH) VALUES (:sip, :yay, :reduk, 'Onaylandý', :onay_tarih)";

                            using (OracleCommand insertCommand = new OracleCommand(insertQuery, connection))
                            {
                                // Parametreleri ekleyin ve deðerlerini ayarlayýn
                                insertCommand.Parameters.Add(new OracleParameter("sip", onay.sip));
                                insertCommand.Parameters.Add(new OracleParameter("yay", onay.yay));
                                insertCommand.Parameters.Add(new OracleParameter("reduk", onay.reduk));
                                insertCommand.Parameters.Add(new OracleParameter("onay_tarih", DateTime.Now));
                                int rowsAffected = insertCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    respons.Response = "Onaylandý";
                                }
                                else
                                {
                                    respons.Response = "Onaylanmadý";
                                }
                            }

                        }
                    }
                }
            }

            return respons;
        }
        [HttpPost]
        [Route("GrafikListele")]
        public List<GrafikModel> Grafik(GrafikGirilen girilen)
        {
            
            OracleConnection con = new OracleConnection(ConnectionString);
            DataTable dt = new DataTable();
            con.Open();
            OracleDataAdapter da = new OracleDataAdapter($@"
        SELECT 
            CASE EXTRACT(MONTH FROM teslim_tarihi)
                WHEN 1 THEN 'Ocak'
                WHEN 2 THEN 'Þubat'
                WHEN 3 THEN 'Mart'
                WHEN 4 THEN 'Nisan'
                WHEN 5 THEN 'Mayýs'
                WHEN 6 THEN 'Haziran'
                WHEN 7 THEN 'Temmuz'
                WHEN 8 THEN 'Aðustos'
                WHEN 9 THEN 'Eylül'
                WHEN 10 THEN 'Ekim'
                WHEN 11 THEN 'Kasým'
                WHEN 12 THEN 'Aralýk'
            END AS Ay,
            SUM(CASE WHEN EXTRACT(YEAR FROM teslim_tarihi) = '{girilen.birinciyil}' THEN Toplam_Siparis_Tutar ELSE 0 END) AS Toplam_Tutar1,
            SUM(CASE WHEN EXTRACT(YEAR FROM teslim_tarihi) = '{girilen.ikinciyil}' THEN Toplam_Siparis_Tutar ELSE 0 END) AS Toplam_Tutar2
        FROM YR_KERIMUSTA3
        WHERE EXTRACT(YEAR FROM teslim_tarihi) = '{girilen.birinciyil}' OR EXTRACT(YEAR FROM teslim_tarihi) = '{girilen.ikinciyil}'
        GROUP BY EXTRACT(MONTH FROM teslim_tarihi)
        ORDER BY EXTRACT(MONTH FROM teslim_tarihi)", con);

            // DataTable verisini al
            da.Fill(dt);

            // Ay ve toplam sipariþ tutarlarýný içeren bir liste oluþtur
            List<GrafikModel> model = new List<GrafikModel>();
            foreach (DataRow row in dt.Rows)
            {
                GrafikModel item = new GrafikModel();
                item.AY = row["Ay"].ToString(); // Ay
                item.TOPLAM1 = row["Toplam_Tutar1"].ToString(); // Ýlk yýlýn toplam tutarý
                item.TOPLAM2 = row["Toplam_Tutar2"].ToString(); // Ýkinci yýlýn toplam tutarý
                model.Add(item);
            }

            // ComboModel listesini JSON olarak dön
            return model;
        }

        [HttpPost]
        [Route("Login")]
        public Responen Login(Log onay)
        {
            Responen respons = new Responen();
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    string sqlQuery = "SELECT SIPARISI_ALAN,ADET FROM Yr_Kerimusta WHERE SIPARISI_ALAN = :username and ADET = :password";

                    using (OracleCommand selectCommand = new OracleCommand(sqlQuery, connection))
                    {
                        selectCommand.Parameters.Add(new OracleParameter("username", onay.username));
                        selectCommand.Parameters.Add(new OracleParameter("password", onay.password));

                        OracleDataReader reader = selectCommand.ExecuteReader();
                        if (reader.Read())
                        {
                            respons.Response = "Giriþ Baþarýlý";
                        }
                        else
                        {
                            respons.Response = "Giriþ Baþarýsýz"; // Kullanýcý bulunamadý veya parola yanlýþ
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Hata iþleme burada yapýlabilir.
                    respons.Response = "Hata: " + ex.Message;
                }
            }
            return respons;
        }
    }
}
