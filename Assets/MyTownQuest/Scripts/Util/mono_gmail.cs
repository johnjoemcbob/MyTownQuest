using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

// From: https://answers.unity.com/questions/433283/how-to-send-email-with-c.html?_ga=2.81718035.568033321.1584179741-152017592.1584179741
public class mono_gmail
{
	private static string Address = "";
	private static string Password = "";
	private static string Product = "";

	public static void Init()
	{
		Address = ( Resources.Load( "Text Assets/email_user" ) as TextAsset ).text;
		Password = ( Resources.Load( "Text Assets/email_pass" ) as TextAsset ).text;
		Product = Application.productName;
	}

	public static void SendMail( string msg, string body = "" )
	{
		if ( Address == "" )
		{
			Init();
		}

		MailMessage mail = new MailMessage();

		mail.From = new MailAddress( Address );
		mail.To.Add( Address );
		mail.Subject = Product +  " - " + msg;
		mail.Body = body;

		SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
		smtpServer.Port = 587;
		smtpServer.Credentials = new System.Net.NetworkCredential( Address, Password ) as ICredentialsByHost;
		smtpServer.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback =
			delegate ( object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors )
			{ return true; };
		smtpServer.Send( mail );
	}
}