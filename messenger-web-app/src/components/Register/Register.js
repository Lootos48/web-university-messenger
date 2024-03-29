import css from "./Register.module.css";

const Register = () => {
    if (sessionStorage.getItem("userID") != null) {
        window.location.assign("http://localhost:3000/mainPage");
    }

    return (
        <div className={css["register"]}>
            <div className={css["register__messenger-icon"]}><img src="./img/messenger-icon.svg" alt="icon" /></div>
            <h1 className={css["register__title"]}>Register your new account</h1>
            <form className={css["register__form"]}>
                <input required className={css["register__input"]} id="register__user-name-input" type="text" maxLength="20" placeholder="Enter your user name" />
                <input required className={css["register__input"]} id="register__password-input" type="password" maxLength="30" placeholder="Enter your password" />
                <button className={css["register__submit-button"]} type="submit">Register</button>
            </form>
        </div>
    );
}

window.addEventListener("click", (event) => {
    const target = event.target.closest("." + css["register__submit-button"]);
    if (target) {
        event.preventDefault();

        const registerQueryObj = {
            username: document.body.querySelector("#register__user-name-input").value,
            password: document.body.querySelector("#register__password-input").value,
        }

        fetch("http://localhost:4000/users/register", {
            method: "post",
            body: JSON.stringify(registerQueryObj),
            headers: {
                "Content-Type": "application/json"
            }
        })
            .then(response => {
                if (response.ok) { return response.json(); }
                else { throw new Error(response.statusText) }
            })
            .then(response => {
                if (response.userId != null) {
                    sessionStorage.setItem("userID", response.userId);
                    sessionStorage.setItem("userName", document.body.querySelector("#register__user-name-input").value);
                    window.location.assign("http://localhost:3000/mainPage");
                }
                else {
                    alert("Reject reason: " + response.error);
                }
            })
            .catch(error => {
                alert(error);
            })
    }
});

export default Register;