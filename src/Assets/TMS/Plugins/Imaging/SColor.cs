namespace TMS.Common.Imaging
{
	public class SColor
	{
		public float A;
		public float B;
		public float G;
		public float R;

		public SColor(float r, float g, float b, float a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public void Add(SColor c)
		{
			R += c.R;
			G += c.G;
			B += c.B;
			A += c.A;
		}

		public void Divide(float divisor)
		{
			R = R/divisor;
			G = G/divisor;
			B = B/divisor;
			A = A/divisor;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SColor))
			{
				return base.Equals(obj);
			}

			var c = (SColor)obj;
			var res = c.R == R &&
			       c.R == G &&
			       c.R == B &&
			       c.R == A;
			return res;
		}

		public override int GetHashCode()
		{
			var o = (int) (A*255);
			o += (int) (G*255) << 8;
			o += (int) (B*255) << 16;
			o += (int) (R*255) << 24;
			return o;
		}
	}
}