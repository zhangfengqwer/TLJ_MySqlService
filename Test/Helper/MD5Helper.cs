using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Model
{
	public static class MD5Helper
	{
		public static string FileMD5(string filePath)
		{
		    using (FileStream file = new FileStream(filePath, FileMode.Open))
			{
				MD5 md5 = new MD5CryptoServiceProvider();
				var bytes = md5.ComputeHash(file);
			    StringBuilder sb = new StringBuilder();

			    foreach (var b in bytes)
			    {
			        sb.Append(b.ToString("x2"));
			    }
			    return sb.ToString();
            }
		}

	    public static string GetMD5(string data)
	    {
	        MD5 md5 = MD5.Create(); //实例化一个md5对像
	        // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
	        byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
	        // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
	        StringBuilder sb = new StringBuilder();

	        foreach (var b in bytes)
	        {
	            sb.Append(b.ToString("x2"));
	        }
	        return sb.ToString();
	        
        }
	}
}
