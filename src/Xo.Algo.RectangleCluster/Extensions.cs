namespace Xo.Algo.RectangleCluster.Extensions;

public static class Extensions
{
	public static int SumWidths(this IEnumerable<IRectangle> @this) => @this.Sum(r => r.W);
	public static int LengthWouldBeWhenAdd(this IEnumerable<IRectangle> @this, int more) => @this.SumWidths() + more;
	public static int RowHeight(this IEnumerable<IRectangle> @this, bool validate = false) => @this.First().H;
	public static IRectangle MapToRectangle(this IRectangleBlueprint @this)
		=> new Rectangle
		{
			W = @this.MinW,
			H = @this.MinH,
			IsElasticH = @this.IsElasticH,
			IsElasticW = @this.IsElasticW,
			Uuid = @this.Uuid,
			GroupId = @this.GroupId
		};

	public static void InflateWidthsToMeet(
		this IEnumerable<IRectangle> @this,
		int width,
		int elasticityH
	)
	{
		if (@this.All(r => !r.IsElasticW)) return;
		if (!@this.All(r => r.W < elasticityH)) return;

		int diff = width - @this.SumWidths();
		if (diff == 0) return;

		while (diff > 0)
		{
			foreach (var r in @this)
			{
				if (diff <= 0 || !r.IsElasticW || r.W == elasticityH) continue;

				r.W++;
				diff--;
			}
		}
	}

	public static void InflateHeightsToHighest(this IEnumerable<IRectangle> @this)
	{
		if (@this.All(r => !r.IsElasticH)) return;

		int highest = @this.Max(r => r.H);

		foreach (var r in @this)
		{
			if (r.H <= highest && !r.IsElasticH) continue;
			r.H += highest - r.H;
		}
	}
}
