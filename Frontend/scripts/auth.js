// Import Firebase
    import { initializeApp } from "https://www.gstatic.com/firebasejs/10.13.1/firebase-app.js";
    import { getAuth, signOut, createUserWithEmailAndPassword, 
             signInWithEmailAndPassword, sendEmailVerification } 
             from "https://www.gstatic.com/firebasejs/10.13.1/firebase-auth.js";
    import { getFirestore, setDoc, getDoc, doc } 
             from "https://www.gstatic.com/firebasejs/10.13.1/firebase-firestore.js";
    

    //Firebase keys
    const firebaseConfig = {
      apiKey: "AIzaSyBJG7c7GtI4fNOZ4ipxxHxrhvCsOE7MN6M",
      authDomain: "practise-d5653.firebaseapp.com",
      projectId: "practise-d5653",
      storageBucket: "practise-d5653.appspot.com",
      messagingSenderId: "672058692738",
      appId: "1:672058692738:web:952434f2599baa419d909b"
    };

    // Initialize Firebase
    const app = initializeApp(firebaseConfig);
    const auth = getAuth(app);
    const db = getFirestore(app);

    // Toggle UI
    function toggleForm(mode) {
      document.getElementById("registerForm").style.display = (mode === "register") ? "block" : "none";
      document.getElementById("loginForm").style.display = (mode === "login") ? "block" : "none";
    }
    window.toggleForm = toggleForm;

    // Register
    async function register() {
      const email = document.getElementById("regEmail").value;
      const password = document.getElementById("regPassword").value;
      const role = document.getElementById("regRole").value;

      try {
        const userCred = await createUserWithEmailAndPassword(auth, email, password);
        await setDoc(doc(db, "users", userCred.user.uid), { role: role, email: email });
        await sendEmailVerification(userCred.user);
        alert("Verification email sent. Please check your inbox.");
      } catch (error) {
        alert(error.message);
      }
    }
    window.register = register;

    // Login
    async function login() {
      const email = document.getElementById("logEmail").value;
      const password = document.getElementById("logPassword").value;

      const auth = getAuth();
      const user = auth.currentUser;

      try {
        const userCred = await signInWithEmailAndPassword(auth, email, password);
        if (userCred.user.emailVerified && user) {
          const userDoc = await getDoc(doc(db, "users", userCred.user.uid));
          const role = userDoc.data().role;
          localStorage.setItem("userRole", role);
          localStorage.setItem("userEmail", email);

          if (role == "creator") {window.location.href = "CRhome.html";}
          if (role == "student") {window.location.href = "SThome.html";}

        } else {
          alert("Please verify your email first!");
        }
      } catch (error) {
        alert(error.message);
      }
    }
    window.login = login;

    // Logout
    function logout() {
  signOut(auth).then(() => {
    // Clear localStorage as well, to fully log out
    localStorage.removeItem("userRole");
    localStorage.removeItem("userEmail");

    window.location.href = "/Frontend/index.html";
  }).catch((error) => {
    alert(error.message);
  });
}
window.logout = logout;

async function getToken() {
  const user = auth.currentUser;
  if (user) {
    const token = await user.getIdToken(); // <-- This is the Firebase ID token (JWT)
    console.log("User token:", token);
    return token;
  } else {
    console.log("No user signed in");
    return null;
  }
}

// Example usage: const token = await user.getIdToken(true);