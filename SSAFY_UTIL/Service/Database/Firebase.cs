using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SSAFY_UTIL.Service.Database
{
    class Firebase : Database
    {
        private static readonly Lazy<Firebase> lazy = new Lazy<Firebase>(() => new Firebase());
        private static Firebase Instance { get { return lazy.Value; } }
        private Firebase() {
            Connect();
        }

        private FirestoreDb db;

        public static Firebase GetInstance()
        {
            return Instance;
        }

        private void Connect()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Secure", "firebaseAccountKey.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            db = FirestoreDb.Create("ssafy-util");
        }

        private bool IsDocument(string Identifier)
        {
            return Identifier.Split('/').Length % 2 == 0;
        }

        public async Task<T> Create<T>(string DataIdentifier, T data)
        {
            DocumentReference doc;

            if (IsDocument(DataIdentifier))
            {
                doc = db.Document(DataIdentifier);
            }
            else
            {
                doc = db.Collection(DataIdentifier).Document();
            }

            await doc.SetAsync(data);
            return data;
        }

        public async Task<List<T>> Read<T>(string DataIdentifier, uint limit = 0)
        {
            List<T> result = default;
            if (IsDocument(DataIdentifier))
            {
                DocumentReference doc = db.Document(DataIdentifier);
                DocumentSnapshot snapshot = await doc.GetSnapshotAsync();
                result.Add(snapshot.ConvertTo<T>());
            }
            else
            {
                CollectionReference collection = db.Collection(DataIdentifier);
                QuerySnapshot snapshot;
                if (limit != 0)
                    snapshot = await collection.Limit((int)limit).GetSnapshotAsync();
                else
                    snapshot = await collection.GetSnapshotAsync();

                foreach (DocumentSnapshot document in snapshot)
                {
                    result.Add(document.ConvertTo<T>());
                }
            }

            return result;
        }

        public async Task<bool> Update(string DataIdentifier, Dictionary<string, object> data)
        {
            if (!IsDocument(DataIdentifier))
                throw new Exception("Data should be a document");

            DocumentReference doc = db.Document(DataIdentifier);
            await doc.UpdateAsync(data);
            return true;
        }

        public async Task<bool> Delete(string DataIdentifier)
        {
            if (IsDocument(DataIdentifier))
            {
                DocumentReference doc = db.Document(DataIdentifier);
                await doc.DeleteAsync();
            }
            else
            {
                const int LIMIT = 100;
                CollectionReference collection = db.Collection(DataIdentifier);
                QuerySnapshot snapshot = await collection.Limit(LIMIT).GetSnapshotAsync();
                IReadOnlyList<DocumentSnapshot> documents = snapshot.Documents;
                while (documents.Count > 0)
                {
                    foreach (DocumentSnapshot document in documents)
                    {
                        await document.Reference.DeleteAsync();
                    }
                    snapshot = await collection.Limit(LIMIT).GetSnapshotAsync();
                    documents = snapshot.Documents;
                }
            }
            return true;
        }
    }
}
