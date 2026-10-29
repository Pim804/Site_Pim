document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('form[data-required-markers] [required]').forEach((field) => {
    const label = field.id && document.querySelector(`label[for="${CSS.escape(field.id)}"]`);
    if (!label || label.querySelector('.required-mark')) return;
    const marker = document.createElement('span');
    marker.className = 'required-mark';
    marker.setAttribute('aria-hidden', 'true');
    marker.textContent = ' *';
    label.append(marker);
  });
  document.querySelectorAll('[data-part-select]').forEach((select) => {
    select.dispatchEvent(new Event('change', { bubbles: true }));
  });
});

document.addEventListener('submit', (event) => {
  const form = event.target;
  if (!(form instanceof HTMLFormElement)) return;
  let message = form.dataset.confirmSubmit;
  const partSelect = form.querySelector('[data-part-select]');
  const quantityInput = form.querySelector('[data-movement-quantity]');
  if (partSelect && quantityInput) {
    const selectedPart = partSelect.selectedOptions[0];
    const balance = Number(selectedPart?.dataset.stock);
    const quantity = Number(quantityInput.value);
    const type = form.querySelector('[name="Type"]')?.value;
    const error = form.querySelector('[data-movement-client-error]');
    if (type === 'exit' && quantity > balance) {
      event.preventDefault();
      if (error) {
        error.textContent = `A saída solicitada (${quantity}) excede o saldo disponível (${balance}).`;
        error.hidden = false;
      }
      quantityInput.focus();
      return;
    }
    if (error) error.hidden = true;
    const nextBalance = type === 'entry' ? balance + quantity : balance - quantity;
    message = `Confirmar ${type === 'entry' ? 'entrada' : 'saída'} de ${quantity} unidade(s) de ${selectedPart.textContent.split(' · ')[1]?.split(' (saldo:')[0] ?? 'peça'}? Saldo: ${balance} → ${nextBalance}.`;
  }
  if (message && !window.confirm(message)) {
    event.preventDefault();
    return;
  }
  if (!form.hasAttribute('data-loading-form')) return;
  const submitter = event.submitter;
  if (!(submitter instanceof HTMLButtonElement) || submitter.dataset.busy === 'true') {
    if (submitter?.dataset.busy === 'true') event.preventDefault();
    return;
  }
  submitter.dataset.busy = 'true';
  submitter.disabled = true;
  if (submitter.dataset.loadingLabel) submitter.textContent = submitter.dataset.loadingLabel;
  form.setAttribute('aria-busy', 'true');
}, true);

document.addEventListener('click', (event) => {
  const dismiss = event.target.closest('.toast-dismiss');
  if (dismiss) dismiss.closest('.toast-message')?.remove();
  const toggle = event.target.closest('[data-password-toggle]');
  if (toggle) {
    const input = toggle.closest('.login-input-wrap')?.querySelector('input');
    if (!input) return;
    const showing = input.type === 'password';
    input.type = showing ? 'text' : 'password';
    toggle.textContent = showing ? 'Ocultar' : 'Mostrar';
    toggle.setAttribute('aria-label', showing ? 'Ocultar senha' : 'Mostrar senha');
    toggle.setAttribute('aria-pressed', String(showing));
  }
});

document.addEventListener('change', (event) => {
  const select = event.target.closest('[data-part-select]');
  if (!select) return;
  const option = select.selectedOptions[0];
  const stock = option?.dataset.stock;
  const output = select.parentElement.querySelector('[data-current-stock]');
  if (output) output.textContent = stock === undefined
    ? 'Selecione uma peça para consultar o saldo atual.'
    : `Saldo atual: ${stock} unidade(s). A API validará o saldo novamente ao registrar.`;
});
