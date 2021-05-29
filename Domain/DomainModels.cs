using System;

namespace MyRental
{
    public record TrackName
    {
        public static Result<TrackName> TryCreate(string Value) =>
          string.IsNullOrEmpty(Value)
              ? Result<TrackName>.Failure("Artist name can not be empty.")
              : Result<TrackName>.Succeed(new TrackName(Value));

        public string Value { get; init; }

        private TrackName(string value)
        {
            Value = value;
        }
    }
    public record ArtistName
    {
        public string Value { get; init; }

        public static Result<ArtistName> TryCreate(string value) =>
            string.IsNullOrEmpty(value)
                ? Result<ArtistName>.Failure("Artist name can not be empty.")
                : Result<ArtistName>.Succeed(new ArtistName(value));

        private ArtistName(string value) { }
    }

    public record Year(int Value);
    public record TrackId(Guid Value);
}