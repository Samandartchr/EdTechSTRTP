const btnDrafts = document.getElementById("show-drafts");
const btnPublished = document.getElementById("show-published");
const drafts = document.getElementById("drafts");
const published = document.getElementById("published");

function activate(show, hide, activeBtn, inactiveBtn) {
  show.style.display = "block";
  hide.style.display = "none";
  activeBtn.style.background = "lightgray";
  inactiveBtn.style.background = "";
}

btnDrafts.addEventListener("click", () =>
  activate(drafts, published, btnDrafts, btnPublished)
);
btnPublished.addEventListener("click", () =>
  activate(published, drafts, btnPublished, btnDrafts)
);

// default state
activate(drafts, published, btnDrafts, btnPublished);

