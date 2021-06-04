using System;

namespace MyRental
{
    public record TrackName
    {
        public string Value { get; init; }
        private TrackName(string value) => Value = value;

        public static Result<TrackName> TryCreate(string value) =>
            value.IsEmpty()
                ? Result<TrackName>.Failure(ValidationError.Create(ErrorType.TrackNameEmpty))
                : Result<TrackName>.Succeed(new TrackName(value));
    }

    public record ArtistName
    {
        public string Value { get; init; }
        private ArtistName(string value) => Value = value;

        public static Result<ArtistName> TryCreate(string value) =>
            value.IsEmpty()
                ? Result<ArtistName>.Failure(ValidationError.Create(ErrorType.ArtistNameEmpty))
                : Result<ArtistName>.Succeed(new ArtistName(value));
    }

    public record Year(int Value);
    public record TrackId(Guid Value);
}