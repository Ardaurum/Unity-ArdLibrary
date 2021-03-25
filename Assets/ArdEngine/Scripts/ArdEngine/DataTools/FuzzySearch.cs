using UnityEngine;

namespace ArdEngine.DataTools
{
    public static class FuzzySearch
    {
        public static int GetSubstringDamerauLevenshteinDistance(this string text, string query)
        {
            (bool isEmpty, int emptyDistance) = DistanceIfEmpty(text, query);
            if (isEmpty)
            {
                return emptyDistance;
            }

            text = text.Trim().ToLower();
            query = query.Trim().ToLower();
            var matrix = new DistanceMatrix(text.Length, query.Length);
            
            for (var h = 0; h < matrix.Height; h++)
            {
                matrix[h, 0] = h;
            }

            matrix = CalculateDistances(text, query, matrix);

            int min = matrix[query.Length, 0];
            for (var i = 1; i < matrix.Width; i++)
            {
                if (matrix[query.Length, i] < min)
                {
                    min = matrix[query.Length, i];
                }
            }

            return min;
        }

        public static int GetDamerauLevenshteinDistance(this string stringA, string stringB)
        {
            (bool isEmpty, int emptyDistance) = DistanceIfEmpty(stringA, stringB);
            if (isEmpty)
            {
                return emptyDistance;
            }
            stringA = stringA.Trim().ToLower();
            stringB = stringB.Trim().ToLower();
            var matrix = new DistanceMatrix(stringA.Length, stringB.Length);

            for (var h = 0; h < matrix.Height; h++)
            {
                matrix[h, 0] = h;
            }
            for (var w = 0; w < matrix.Width; w++)
            {
                matrix[0, w] = w;
            }

            matrix = CalculateDistances(stringA, stringB, matrix);
            return matrix[matrix.Height - 1, matrix.Width - 1];
        }
        
        private static (bool Empty, int Distance) DistanceIfEmpty(string stringA, string stringB)
        {
            if (string.IsNullOrEmpty(stringA))
            {
                return (true, string.IsNullOrEmpty(stringB) ? 0 : stringB.Length);
            }

            if (string.IsNullOrEmpty(stringB))
            {
                return (true, string.IsNullOrEmpty(stringA) ? 0 : stringA.Length);
            }

            return (false, 0);
        }
        
        private static DistanceMatrix CalculateDistances(string stringA, string stringB, DistanceMatrix matrix)
        {
            for (var h = 1; h < matrix.Height; h++)
            {
                for (var w = 1; w < matrix.Width; w++)
                {
                    int cost = stringB[h - 1] == stringA[w - 1] ? 0 : 1;
                    int insertion = matrix[h, w - 1] + 1;
                    int deletion = matrix[h - 1, w] + 1;
                    int substitution = matrix[h - 1, w - 1] + cost;

                    int distance = Mathf.Min(insertion, Mathf.Min(deletion, substitution));

                    if (h > 1 && w > 1 && stringB[h - 1] == stringA[w - 2] && stringB[h - 2] == stringA[w - 1])
                    {
                        distance = Mathf.Min(distance, matrix[h - 2, w - 2] + cost);
                    }

                    matrix[h, w] = distance;
                }
            }

            return matrix;
        }

        private readonly struct DistanceMatrix
        {
            public readonly int Width;
            public readonly int Height;
            public readonly int[,] Matrix;

            public int this[int i, int j]
            {
                get => Matrix[i, j];
                set => Matrix[i, j] = value;
            }

            public DistanceMatrix(int width, int height)
            {
                Width = width + 1;
                Height = height + 1;
                Matrix = new int[Height, Width];
            }
        }
    }
}