namespace Immediate.Apis.Generators;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public static class EquatableDictionary
{
	public static EquatableDictionary<TKey, TValue> ToEquatableDictionary<TKey, TValue>(
		this IReadOnlyDictionary<TKey, TValue>? dict
	)
		where TValue : IEquatable<TValue>
	{
		return new(dict);
	}
}

public readonly struct EquatableDictionary<TKey, TValue>(
	IReadOnlyDictionary<TKey, TValue>? dictionary
) : IEquatable<EquatableDictionary<TKey, TValue>>
		where TValue : IEquatable<TValue>
{
	private readonly IReadOnlyDictionary<TKey, TValue>? _dictionary = dictionary;
	private readonly int _hashCode = BuildHashCode(dictionary);

	private static int BuildHashCode(IReadOnlyDictionary<TKey, TValue>? dictionary)
	{
		if (dictionary is null)
			return new HashCode().ToHashCode();

		var hashCode = new HashCode();

		foreach (var kvp in dictionary.OrderBy(kvp => kvp.Key))
		{
			hashCode.Add(kvp.Key);
			hashCode.Add(kvp.Value);
		}

		return hashCode.ToHashCode();
	}

	public bool Equals(EquatableDictionary<TKey, TValue> other) =>
		ReferenceEquals(_dictionary, other._dictionary)
			|| (GetHashCode() == other.GetHashCode()
				// both null is covered by reference equals
				// only one null covered by different hash codes
				&& _dictionary!.Count == other._dictionary!.Count
				&& _dictionary
					.All(kvp =>
						other._dictionary.TryGetValue(kvp.Key, out var oValue)
							&& EqualityComparer<TValue>.Default.Equals(kvp.Value, oValue)
					)
				);

	public override bool Equals(object obj) =>
		obj is EquatableDictionary<TKey, TValue> dict && Equals(dict);

	public override int GetHashCode() => _hashCode;

	public static bool operator ==(EquatableDictionary<TKey, TValue> left, EquatableDictionary<TKey, TValue> right) =>
		left.Equals(right);

	public static bool operator !=(EquatableDictionary<TKey, TValue> left, EquatableDictionary<TKey, TValue> right) =>
		!(left == right);

	public bool TryGetValue(TKey key, out TValue value)
	{
		if (_dictionary is { })
			return _dictionary.TryGetValue(key, out value);

		value = default!;
		return false;
	}

	public TValue GetValueOrDefault(TKey key)
	{
		if (_dictionary is { } && _dictionary.TryGetValue(key, out var value))
			return value;

		return default!;
	}
}
