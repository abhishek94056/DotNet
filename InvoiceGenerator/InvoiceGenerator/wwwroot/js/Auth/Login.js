   function togglePwd() {
        const inp  = document.getElementById('passwordInput');
    const icon = document.getElementById('eyeIcon');
    if (inp.type === 'password') {
        inp.type = 'text';
    icon.className = 'bi bi-eye-slash';
        } else {
        inp.type = 'password';
    icon.className = 'bi bi-eye';
        }
    }