using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Examyoss.Services
{
        public class Conexionfirebase
        {
            public static FirebaseClient firebase = new FirebaseClient("https://examen-3parcial-default-rtdb.firebaseio.com/");

            public const string WebapyFirebase = "AIzaSyDmE9sykyxBS5sWsmO4VeiEvZEea7o-7EY";
        }
}
