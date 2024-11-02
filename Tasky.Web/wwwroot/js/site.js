let timeout = null;
function relentizarSubmit() {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
        document.getElementById('filtroForm').submit();
    }, 500);
}