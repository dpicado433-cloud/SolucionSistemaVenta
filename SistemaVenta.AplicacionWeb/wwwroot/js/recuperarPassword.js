document.addEventListener("DOMContentLoaded", function () {
    const btnRecuperar = document.getElementById("btnRecuperar");

    if (btnRecuperar) {
        btnRecuperar.addEventListener("click", async () => {
            const correo = document.getElementById("txtCorreoRecuperar").value;

            if (!correo) {
                alert("Por favor, ingresa tu correo electrónico.");
                return;
            }

            try {
                const client = new Appwrite.Client()
                    .setEndpoint('https://nyc.cloud.appwrite.io/v1')
                    .setProject('6ace3a1d001dd7fc40f7');       

                const account = new Appwrite.Account(client);

                await account.createRecovery(
                    correo, 
                    'http://localhost:60294/Consulta/RestablecerContrasena'
                );
                
                alert("¡Correo de recuperación enviado con éxito! Revisa tu bandeja de entrada.");
            } catch (error) {
                console.error("Error al enviar recuperación:", error);
                alert("Ocurrió un error: " + error.message);
            }
        });
    }
});