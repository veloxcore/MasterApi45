using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;

namespace BarcoderApp.RestApi.Providers
{
    /// <summary>
    /// This formats the token.
    /// </summary>
    public class JWTAccessTokenFormater : ISecureDataFormat<AuthenticationTicket>
    {
        private string _issuer = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="JWTAccessTokenFormater"/> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        public JWTAccessTokenFormater(string issuer)
        {
            _issuer = issuer;
        }


        /// <summary>
        /// Protects the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            string audienceId = ConfigurationManager.AppSettings["jwt:AudienceId"];
            string symmetricKeyAsBase64 = ConfigurationManager.AppSettings["jwt:AudienceSecret"];
            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
            var signingKey = new HmacSigningCredentials(keyByteArray);
            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;
            var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);
            return jwt;
        }

        /// <summary>
        /// Unprotects the specified protected text.
        /// </summary>
        /// <param name="protectedText">The protected text.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Check for the signing credentials
    /// </summary>
    /// <seealso cref="System.IdentityModel.Tokens.SigningCredentials" />
    public class HmacSigningCredentials : SigningCredentials
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="HmacSigningCredentials"/> class.
        /// </summary>
        /// <param name="base64EncodedKey">The base64 encoded key.</param>
        public HmacSigningCredentials(string base64EncodedKey) : this(Convert.FromBase64String(base64EncodedKey))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HmacSigningCredentials"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public HmacSigningCredentials(byte[] key) : base(new InMemorySymmetricSecurityKey(key), CreateSignatureAlgorithm(key), CreateDigestAlgorithm(key))
        {

        }

        /// <summary>
        /// Creates the signature algorithm.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Unsupported key length</exception>
        protected static string CreateSignatureAlgorithm(byte[] key)
        {
            switch (key.Length)
            {
                case 32:
                    return Algorithms.HmacSha256Signature;
                case 48:
                    return Algorithms.HmacSha384Signature;
                case 64:
                    return Algorithms.HmacSha512Signature;
                default:
                    throw new InvalidOperationException("Unsupported key length");
            }
        }

        /// <summary>
        /// Creates the digest algorithm.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Unsupported key length</exception>
        protected static string CreateDigestAlgorithm(byte[] key)
        {
            switch (key.Length)
            {
                case 32:
                    return Algorithms.Sha256Digest;
                case 48:
                    return Algorithms.Sha384Digest;
                case 64:
                    return Algorithms.Sha512Digest;
                default:
                    throw new InvalidOperationException("Unsupported key length");
            }
        }
    }

    /// <summary>
    /// Provides hashing algorithms.
    /// </summary>
    public sealed class Algorithms
    {
        private Algorithms()
        {
        }
        /// <summary>
        /// The hmac sha256 signature
        /// </summary>
        public const string HmacSha256Signature = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
        /// <summary>
        /// The hmac sha384 signature
        /// </summary>
        public const string HmacSha384Signature = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";

        /// <summary>
        /// The hmac sha512 signature
        /// </summary>
        public const string HmacSha512Signature = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
        /// <summary>
        /// The sha256 digest
        /// </summary>
        public const string Sha256Digest = "http://www.w3.org/2001/04/xmlenc#sha256";
        /// <summary>
        /// The sha384 digest
        /// </summary>
        public const string Sha384Digest = "http://www.w3.org/2001/04/xmlenc#sha384";
        /// <summary>
        /// The sha512 digest
        /// </summary>
        public const string Sha512Digest = "http://www.w3.org/2001/04/xmlenc#sha512";
    }

}