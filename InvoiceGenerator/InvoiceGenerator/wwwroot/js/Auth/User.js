    // ════════════════════════════════════════════
    // PAGINATION STATE
    // ════════════════════════════════════════════
    let currentPage  = 1;
    let rowsPerPage  = 15;
    let totalPages   = 1;
    let filteredRows = [];

    const statusModal = new bootstrap.Modal(document.getElementById('statusModal'));
    const toastEl     = document.getElementById('toast');
    const bsToast     = new bootstrap.Toast(toastEl, { delay: 3000 });

    let selectedUserId = 0;
    let selectedStatus = false;

    // ════════════════════════════════════════════
    // PAGINATION FUNCTIONS
    // ════════════════════════════════════════════
    function getAllRows() {
        return Array.from(document.querySelectorAll('#tableBody tr'));
    }

    function filterTable() {
        const q = document.getElementById('searchInput').value.toLowerCase().trim();
        filteredRows = getAllRows().filter(row =>
            !q || row.textContent.toLowerCase().includes(q)
        );
        currentPage = 1;
        renderPage();
    }

    function renderPage() {
        // Hide all
        getAllRows().forEach(row => row.style.display = 'none');

        totalPages  = Math.max(1, Math.ceil(filteredRows.length / rowsPerPage));
        if (currentPage > totalPages) currentPage = totalPages;

        const start = (currentPage - 1) * rowsPerPage;
        const end   = Math.min(start + rowsPerPage, filteredRows.length);

        // Show current page rows
        for (let i = start; i < end; i++)
            filteredRows[i].style.display = '';

        updatePaginationUI(start, end);
    }

    function updatePaginationUI(start, end) {
        document.getElementById('pageInfo').textContent =
            filteredRows.length === 0
                ? 'No records'
                : `${start + 1}–${end} of ${filteredRows.length}`;

        document.getElementById('btnFirst').disabled = currentPage === 1;
        document.getElementById('btnPrev').disabled  = currentPage === 1;
        document.getElementById('btnNext').disabled  = currentPage === totalPages;
        document.getElementById('btnLast').disabled  = currentPage === totalPages;

        // Page number buttons (sliding window of 5)
        const container  = document.getElementById('pageNumbers');
        container.innerHTML = '';

        const maxVisible = 3;
        let   startPage  = Math.max(1, currentPage - Math.floor(maxVisible / 2));
        let   endPage    = Math.min(totalPages, startPage + maxVisible - 1);
        if (endPage - startPage < maxVisible - 1)
            startPage = Math.max(1, endPage - maxVisible + 1);

        for (let p = startPage; p <= endPage; p++) {
            const btn       = document.createElement('button');
            btn.textContent = p;
            btn.className   = 'btn btn-outline-secondary btn-sm'
                            + (p === currentPage ? ' active-page' : '');
            btn.onclick     = () => goToPage(p);
            container.appendChild(btn);
        }
    }

    function goToPage(page) {
        if (page < 1 || page > totalPages) return;
        currentPage = page;
        renderPage();
    }

    function changeRowsPerPage() {
        rowsPerPage = parseInt(
            document.getElementById('rowsPerPageSelect').value);
        currentPage = 1;
        renderPage();
    }

    // Init
    document.addEventListener('DOMContentLoaded', () => {
        filteredRows = getAllRows();
        renderPage();
    });

    // ════════════════════════════════════════════
    // STATUS TOGGLE
    // ════════════════════════════════════════════
    function openStatusModal(userId, activate) {
        selectedUserId = userId;
        selectedStatus = activate;
        document.getElementById('statusMsg').textContent =
            activate
                ? 'Are you sure you want to activate this user?'
                : 'Are you sure you want to deactivate this user?';
        statusModal.show();
    }

    document.getElementById('btnConfirmStatus').addEventListener('click', async () => {
        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]')?.value ?? '';
        try {
            const r = await fetch('/Auth/ToggleActive', {
                method:  'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body:    new URLSearchParams({
                    userId:   selectedUserId,
                    isActive: selectedStatus,
                    __RequestVerificationToken: token
                })
            });
            const d = await r.json();
            statusModal.hide();
            if (d.success) {
                showToast(
                    selectedStatus ? 'User activated.' : 'User deactivated.',
                    'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                showToast('Action failed.', 'danger');
            }
        } catch (err) {
            showToast('Network error.', 'danger');
        }
    });

    // ════════════════════════════════════════════
    // TOAST
    // ════════════════════════════════════════════
    function showToast(msg, type = 'success') {
        toastEl.className =
            `toast align-items-center text-white border-0 bg-${type}`;
        document.getElementById('toastMsg').textContent = msg;
        bsToast.show();
    }