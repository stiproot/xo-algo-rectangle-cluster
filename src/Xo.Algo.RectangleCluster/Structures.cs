namespace Xo.Algo.RectangleCluster.Structures;

public record Rectangle : IRectangle
{
	public int X { get; set; }
	public int Y { get; set; }
	public int W { get; set; }
	public int H { get; set; }
	public bool IsElasticW { get; init; }
	public bool IsElasticH { get; init; }
	public string? Uuid { get; init; }
	public int GroupId { get; init; }
	public bool IsOverlapX(IRectangle r) => this.X + this.W <= r.X || r.X + r.W <= this.X ? false : true;
	public bool IsOverlapY(IRectangle r) => this.Y + this.H <= r.Y || r.Y + r.H <= this.Y ? false : true;
	public bool IsOverlap(IRectangle r) => this.IsOverlapX(r) && this.IsOverlapY(r);
}

public record RectangleBlueprint : IRectangleBlueprint
{
	public int MinH { get; init; }
	public int MinW { get; init; }
	public bool IsElasticW { get; init; }
	public bool IsElasticH { get; init; }
	public string? Uuid { get; init; }
	public int GroupId { get; init; }
}