namespace Xo.Algo.RectangleCluster;

public class Grid : IGrid
{
	const int ELASTICITY_H = 10;
	private readonly int _width;
	private readonly int _elasticityH;
	private readonly List<List<IRectangle>> _output = new List<List<IRectangle>>();
	private IDictionary<string, IRectangle> _hash = new Dictionary<string, IRectangle>();

	public Grid(int width, int elasticityH = ELASTICITY_H) => (this._width, this._elasticityH) = (width, elasticityH);

	public virtual IGrid Init(IEnumerable<IRectangleBlueprint> blueprints)
	{
		var row = new List<IRectangle>();

		foreach (var (b, i) in blueprints.Select((b, i) => (b, i)))
		{
			if (row.LengthWouldBeWhenAdd(b.MinW) <= this._width && (row.LastOrDefault()?.GroupId ?? b.GroupId) == b.GroupId)
			{
				row.Add(b.MapToRectangle());
				if (i == blueprints.Count() - 1) this._output.Add(row);
				continue;
			}

			this._output.Add(row);
			row = new List<IRectangle> { b.MapToRectangle() };
			if (i == blueprints.Count() - 1) this._output.Add(row);
		}

		return this;
	}

	public IGrid Validate()
	{
		var rs = this._output.SelectMany(row => row);

		foreach (var r in rs)
		{
			if (r.W > ELASTICITY_H) throw new InvalidOperationException($"Rectangle {r} exceeds horizontal elasticity limit of {ELASTICITY_H}...");

			foreach (var _r in rs)
			{
				if (r == _r) continue;
				if (r.IsOverlap(_r)) throw new InvalidOperationException($"{r} overlaps with {_r}...");
			}
		}
		return this;
	}

	public IGrid InflateRowWidthsToMeet()
	{
		this._output.ForEach(r => r.InflateWidthsToMeet(this._width, ELASTICITY_H));
		return this;
	}

	public IGrid InflateRowHeightsToHighest()
	{
		this._output.ForEach(r => r.InflateHeightsToHighest());
		return this;
	}

	public virtual IGrid InitCoords()
	{
		int runningH = 0;
		foreach (var row in this._output)
		{
			int runningW = 0;
			foreach (var r in row)
			{
				r.X = runningW;
				r.Y = runningH;
				runningW += r.W;
			}
			runningH += row.RowHeight();
		}

		return this;
	}

	public IGrid Print()
	{
		this._output.ForEach(r => Console.WriteLine(JsonSerializer.Serialize(r)));
		return this;
	}

	public IEnumerable<IEnumerable<IRectangle>> Result() => this._output;

	public IGrid IniHash()
	{
		this._hash = this._output.SelectMany(row => row).ToDictionary(r => r.Uuid!);
		return this;
	}

	public IDictionary<string, IRectangle> Hash() => this._hash;
}
