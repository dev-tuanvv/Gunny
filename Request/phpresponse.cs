namespace Tank.Request
{
    using Bussiness;
    using Bussiness.Managers;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;

    public class phpresponse : IHttpHandler
    {
        public static string DecryptRJ256(string prm_key, string prm_iv, string prm_text_to_decrypt)
        {
            string s = prm_text_to_decrypt;
            RijndaelManaged managed = new RijndaelManaged {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC,
                KeySize = 0x100,
                BlockSize = 0x100
            };
            byte[] bytes = Encoding.ASCII.GetBytes(prm_key);
            byte[] rgbIV = Encoding.ASCII.GetBytes(prm_iv);
            ICryptoTransform transform = managed.CreateDecryptor(bytes, rgbIV);
            byte[] buffer = Convert.FromBase64String(s);
            byte[] buffer4 = new byte[buffer.Length];
            MemoryStream stream = new MemoryStream(buffer);
            new CryptoStream(stream, transform, CryptoStreamMode.Read).Read(buffer4, 0, buffer4.Length);
            return Encoding.ASCII.GetString(buffer4);
        }

        public static string EncryptRJ256(string prm_key, string prm_iv, string prm_text_to_encrypt)
        {
            string s = prm_text_to_encrypt;
            RijndaelManaged managed = new RijndaelManaged {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC,
                KeySize = 0x100,
                BlockSize = 0x100
            };
            byte[] bytes = Encoding.ASCII.GetBytes(prm_key);
            byte[] rgbIV = Encoding.ASCII.GetBytes(prm_iv);
            ICryptoTransform transform = managed.CreateEncryptor(bytes, rgbIV);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            byte[] buffer = Encoding.ASCII.GetBytes(s);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            return Convert.ToBase64String(stream.ToArray());
        }

        public void ProcessRequest(HttpContext context)
        {
            string str = context.Request["phpkey"];
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    context.Response.Write("phpkey");
                }
                else
                {
                    string[] strArray = DecryptRJ256(this.m_key, this.m_iv, str).Split(new char[] { ';' });
                    if (strArray.Length < 3)
                    {
                        context.Response.Write(5);
                    }
                    else
                    {
                        string str3 = strArray[0];
                        string str4 = strArray[1];
                        string[] strArray2 = strArray[2].Split(new char[] { '|' });
                        if (string.IsNullOrEmpty(str3))
                        {
                            context.Response.Write(3);
                        }
                        else if (string.IsNullOrEmpty(str4))
                        {
                            context.Response.Write(6);
                        }
                        else if (strArray2.Length == 0)
                        {
                            context.Response.Write(5);
                        }
                        else if (strArray2.Length > 50)
                        {
                            context.Response.Write(4);
                        }
                        else
                        {
                            string str6 = str3;
                            if ((str6 != null) && (((str6 == "senditembyusername") || (str6 == "senditembyid")) || (str6 == "senditembynickname")))
                            {
                                using (PlayerBussiness bussiness = new PlayerBussiness())
                                {
                                    int num;
                                    PlayerInfo userSingleByUserName = null;
                                    str6 = str3;
                                    if (str6 != null)
                                    {
                                        if (!(str6 == "senditembyusername"))
                                        {
                                            if (str6 == "senditembyid")
                                            {
                                                goto Label_01D9;
                                            }
                                            if (str6 == "senditembynickname")
                                            {
                                                goto Label_01EB;
                                            }
                                        }
                                        else
                                        {
                                            userSingleByUserName = bussiness.GetUserSingleByUserName(str4);
                                        }
                                    }
                                    goto Label_01F8;
                                Label_01D9:
                                    userSingleByUserName = bussiness.GetUserSingleByUserID(int.Parse(str4));
                                    goto Label_01F8;
                                Label_01EB:
                                    userSingleByUserName = bussiness.GetUserSingleByNickName(str4);
                                Label_01F8:
                                    if (userSingleByUserName != null)
                                    {
                                        goto Label_0268;
                                    }
                                    str6 = str3;
                                    if (str6 != null)
                                    {
                                        if (!(str6 == "senditembyusername"))
                                        {
                                            if ((str6 == "senditembynickname") || (str6 == "senditembyid"))
                                            {
                                                goto Label_024E;
                                            }
                                        }
                                        else
                                        {
                                            context.Response.Write(7);
                                        }
                                    }
                                    return;
                                Label_024E:
                                    context.Response.Write(6);
                                    return;
                                Label_0268:
                                    num = 0;
                                    List<ItemInfo> infos = new List<ItemInfo>();
                                    foreach (string str5 in strArray2)
                                    {
                                        string[] strArray3 = str5.Split(new char[] { ',' });
                                        if (strArray3.Length < 8)
                                        {
                                            num++;
                                        }
                                        else
                                        {
                                            ItemTemplateInfo goods = ItemMgr.FindItemTemplate(int.Parse(strArray3[0]));
                                            if (goods == null)
                                            {
                                                num++;
                                            }
                                            else
                                            {
                                                ItemInfo item = ItemInfo.CreateFromTemplate(goods, 1, 0x66);
                                                item.Count = int.Parse(strArray3[1]);
                                                item.StrengthenLevel = int.Parse(strArray3[2]);
                                                item.AttackCompose = int.Parse(strArray3[3]);
                                                item.AgilityCompose = int.Parse(strArray3[4]);
                                                item.LuckCompose = int.Parse(strArray3[5]);
                                                item.DefendCompose = int.Parse(strArray3[6]);
                                                item.ValidDate = int.Parse(strArray3[7]);
                                                item.IsBinds = true;
                                                infos.Add(item);
                                            }
                                        }
                                    }
                                    if (num > 0)
                                    {
                                        context.Response.Write(2);
                                    }
                                    else if (WorldEventMgr.SendItemsToMail(infos, userSingleByUserName.ID, userSingleByUserName.NickName, "Thư từ hệ thống webshop"))
                                    {
                                        context.Response.Write(0);
                                    }
                                    else
                                    {
                                        context.Response.Write(1);
                                    }
                                }
                            }
                            else
                            {
                                context.Response.Write(3);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                context.Response.Write(exception.Message);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string m_iv
        {
            get
            {
                return ConfigurationManager.AppSettings["m_iv"];
            }
        }

        private string m_key
        {
            get
            {
                return ConfigurationManager.AppSettings["m_key"];
            }
        }
    }
}

