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
	public static void SendMail()
	{
		string address = ( Resources.Load( "Text Assets/email_user" ) as TextAsset ).text;
		string pass = ( Resources.Load( "Text Assets/email_pass" ) as TextAsset ).text;

		MailMessage mail = new MailMessage();

		mail.From = new MailAddress( address );
		mail.To.Add( address );
		mail.Subject = "Received new voice feedback for " + Application.productName;
		mail.Body = "";

		SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
		smtpServer.Port = 587;
		smtpServer.Credentials = new System.Net.NetworkCredential( address, pass ) as ICredentialsByHost;
		smtpServer.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback =
			delegate ( object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors )
			{ return true; };
		smtpServer.Send( mail );
		Debug.Log( "success" );

	}
}