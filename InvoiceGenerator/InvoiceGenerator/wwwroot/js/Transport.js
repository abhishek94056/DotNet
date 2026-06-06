
    // ════════════════════════════════════════════
    // PAGINATION STATE
    // ════════════════════════════════════════════
    let currentPage  = 1;
    let rowsPerPage  = 15;
    let totalPages   = 1;
    let filteredRows = [];

    const modal       = new bootstrap.Modal(document.getElementById('transportModal'));
    const deleteModal = new bootstrap.Modal(document.getElementById('deleteModal'));
    const toastEl     = document.getElementById('toast');
    const bsToast     = new bootstrap.Toast(toastEl, { delay: 4000 });
    let   pendingDeleteId = null;

    // ════════════════════════════════════════════
    // PAGINATION FUNCTIONS
    // ════════════════════════════════════════════
    function getAllRows() {
        return Array.from(document.querySelectorAll('#transportTableBody tr'));
    }

    function filterTable() {
        const q = document.getElementById('searchInput').value.toLowerCase().trim();
        filteredRows = getAllRows().filter(row =>
            !q || row.textContent.toLowerCase().includes(q));
        currentPage = 1;
        renderPage();
    }

    function renderPage() {
        getAllRows().forEach(row => row.style.display = 'none');

        totalPages = Math.max(1, Math.ceil(filteredRows.length / rowsPerPage));
        if (currentPage > totalPages) currentPage = totalPages;

        const start = (currentPage - 1) * rowsPerPage;
        const end   = Math.min(start + rowsPerPage, filteredRows.length);

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

        const container  = document.getElementById('pageNumbers');
        container.innerHTML = '';
        const maxVisible = 3;
        let   startPage  = Math.max(1, currentPage - Math.floor(maxVisible / 2));
        let   endPage    = Math.min(totalPages, startPage + maxVisible - 1);
        if (endPage - startPage < maxVisible - 1)
            startPage = Math.max(1, endPage - maxVisible + 1);

        for (let p = startPage; p <= endPage; p++) {
            const btn = document.createElement('button');
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

    document.addEventListener('DOMContentLoaded', () => {
        filteredRows = getAllRows();
        renderPage();
    });

    // ════════════════════════════════════════════
    // CRUD
    // ════════════════════════════════════════════

    // ── Open Add Modal ──
    function openModal() {
        clearForm();
        document.getElementById('modalTitleText').textContent = 'Add Transport Mode';
        modal.show();
    }

    // ── Edit ──
    async function editMode(id) {
        try {
            const r = await fetch(`/Transport/GetById?Id=${id}`);
            const d = await r.json();
            document.getElementById('modalTitleText').textContent = 'Edit Transport Mode';
            document.getElementById('Id').value       = d.Id;
            document.getElementById('modeName').value = d.ModeName;
            clearErrors();
            modal.show();
        } catch (err) {
            showToast('Failed to load transport data.', 'danger');
        }
    }

    // ── Save ──
    async function saveMode() {
        clearErrors();

        const modeName = document.getElementById('modeName').value.trim();

        if (!modeName) {
            showError('modeName', 'err-modeName', 'Mode Name is required.');
            return;
        }

        const token = document.querySelector(
            '#transportForm input[name="__RequestVerificationToken"]').value;

        const payload = new URLSearchParams({
            Id:       document.getElementById('Id').value,
            ModeName: modeName,
            __RequestVerificationToken: token
        });

        try {
            const r = await fetch('/Transport/Save', {
                method:  'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body:    payload
            });
            const d = await r.json();

            if (!d.success) {
                if (d.errors) d.errors.forEach(e => showToast(e, 'danger'));
                else showToast(d.message || 'Save failed.', 'danger');
                return;
            }

            modal.hide();
            showToast(d.message || 'Transport mode saved successfully!', 'success');
            setTimeout(() => location.reload(), 1200);
        } catch (err) {
            showToast('Network error. Please try again.', 'danger');
        }
    }

    // ── Delete confirm ──
    function deleteMode(id, name) {
        pendingDeleteId = id;
        document.getElementById('deleteModeLabel').textContent = name;
        deleteModal.show();
    }

    document.getElementById('btnConfirmDelete').addEventListener('click', async () => {
        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]').value;
        try {
            const r = await fetch('/Transport/Delete', {
                method:  'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body:    new URLSearchParams({
                    id: pendingDeleteId,
                    __RequestVerificationToken: token
                })
            });
            const d = await r.json();
            deleteModal.hide();
            if (d.success) {
                document.getElementById(`row-${pendingDeleteId}`)?.remove();
                filteredRows = filteredRows.filter(
                    row => row.id !== `row-${pendingDeleteId}`);
                renderPage();
                showToast('Transport mode deleted successfully.', 'success');
            } else {
                showToast('Delete failed.', 'danger');
            }
        } catch (err) {
            showToast('Network error.', 'danger');
        }
    });

    // ════════════════════════════════════════════
    // HELPERS
    // ════════════════════════════════════════════
    function clearForm() {
        document.getElementById('Id').value       = '0';
        document.getElementById('modeName').value = '';
        clearErrors();
    }

    function clearErrors() {
        document.querySelectorAll('.invalid-feedback')
            .forEach(el => { el.textContent = ''; });
        document.querySelectorAll('.form-control, .form-select')
            .forEach(el => el.classList.remove('is-invalid'));
    }

    function showError(fieldId, errId, msg) {
        document.getElementById(fieldId)?.classList.add('is-invalid');
        document.getElementById(errId).textContent = msg;
    }

    function showToast(msg, type = 'success') {
        toastEl.className =
            `toast align-items-center text-white border-0 bg-${type}`;
        document.getElementById('toastMsg').textContent = msg;
        bsToast.show();
    }