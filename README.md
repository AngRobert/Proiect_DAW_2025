# Platformă Online Shop - Proiect ASP.NET Core

Acest proiect reprezintă o platformă de E-Commerce completă, dezvoltată în cadrul laboratorului de **Dezvoltarea Aplicațiilor Web (ASP.NET)**. Aplicația gestionează fluxul complet de vânzare-cumpărare, de la gestionarea produselor de către administratori și colaboratori, până la experiența de cumpărare a clientului, asistată de inteligență artificială.

## 👥 Echipa de Dezvoltare (Grupa 231)
* **Angelescu Paul Robert**
* **Surdu Bogdan-Ionuț**

---

## 🛠️ Arhitectură și Tehnologii
* **Backend:** ASP.NET Core MVC 9.0
* **Baza de date:** SQL Server, Entity Framework Core
* **Autentificare:** ASP.NET Core Identity (Roluri: Admin, Colaborator, User, Vizitator)
* **Frontend:** Razor Views, Bootstrap (inspirat de design-ul eMAG)
* **AI Integration:** Google Gemini API (2.5 flash lite)
* **Versiune Control:** Git & GitHub

---

## 📅 Desfășurarea Proiectului (Sprints & Roadmap)

Proiectul a fost împărțit în 4 Sprint-uri majore, gestionate prin Trello. Mai jos sunt detaliate funcționalitățile implementate și provocările întâmpinate în fiecare etapă.

### 🔹 Etapa 1: Planificare și Setup
* **Provocare:** Estimarea dificultății task-urilor.
* **Soluție:** Am utilizat instrumente AI pentru a sugera o împărțire inițială a task-urilor, pe care am rafinat-o ulterior prin discuții pe Discord.
* **Setup:** Crearea repository-ului, configurarea arhitecturii MVC și schițarea diagramei bazei de date.

### 🚀 Sprint 1: Fundamente
**Funcționalități:**
- [x] Implementare sistem de Roluri (Robert)
- [x] Definirea Modelelor de Date (Bogdan)
- [x] Seeding inițial pentru baza de date (Bogdan)

> **Notă:** Progresul a fost ușor încetinit de configurarea mediului de dezvoltare (Visual Studio, probleme de compatibilitate Windows 11), dar au fost rezolvate eficient.

### 🎨 Sprint 2: CRUD și Frontend
**Funcționalități:**
- [x] CRUD Produse (Robert)
- [x] CRUD Categorii (Bogdan)
- [x] CRUD Review-uri (Bogdan)
- [x] Stocare locală a imaginilor (Robert)
- [x] Implementare UI cu Bootstrap (Design inspirat de eMAG)

> **Lecție învățată (Git):** Am întâmpinat conflicte majore între versiunea locală și cea remote din cauza lipsei de branch-uri separate pentru fix-uri rapide.
> *Soluție:* Am învățat să folosim `git rebase` și am adoptat un workflow strict bazat pe branch-uri pentru a evita conflictele.

### 🛒 Sprint 3: Funcționalități E-Commerce
**Funcționalități:**
- [x] Implementare Wishlist (Bogdan)
- [x] Coș de Cumpărături (Robert)
- [x] Plasare Comenzi Fictive & Scădere Stoc (Robert)
- [x] Actualizare diagramă bază de date (adăugare `Order` și `OrderItem`)

> **Lecție învățată (Backup):** Defecțiunea unui laptop în timpul dezvoltării coșului de cumpărături (înainte de commit) ne-a învățat importanța commit-urilor și push-urilor frecvente ("commit early, commit often"). Datele au fost recuperate, dar riscul a fost major.

### 🧠 Sprint 4: Advanced Features & AI
**Funcționalități:**
- [x] Căutare Produse după denumire parțială (Bogdan)
- [x] Filtrare Produse după categorii (Bogdan)
- [x] Flux Propunere – Aprobare Produse pentru Colaboratori (Robert)
- [x] CRUD Utilizatori - Administrare (Robert)
- [x] **AI Product Assistant** (Bogdan)

---

## 🤖 Componenta AI: "Product Assistant"

Pentru a îmbunătăți experiența utilizatorilor, am integrat un asistent virtual pe pagina fiecărui produs.

### Mod de funcționare:
1.  **Model Utilizat:** Google AI **Gemini 2.5 flash lite**.
2.  **Context:** Asistentul primește automat în prompt **descrierea produsului** și **lista de întrebări frecvente (FAQ)** deja existente în baza de date.
3.  **Interacțiune:**
    * Răspunde la întrebări specifice (ex: *"Are garanție?", "Este potrivit pentru copii?"*).
    * Oferă răspunsuri scurte și la obiect.
    * **Fallback:** Dacă informația nu există în descriere sau în istoricul FAQ, AI-ul este instruit să răspundă onest: *“Momentan nu avem detalii despre acest aspect”*, evitând halucinațiile.
    * Răspunsurile generate sunt salvate în baza de date pentru a antrena viitoarele interacțiuni.

---

## 📝 Concluzii
Acest proiect ne-a oferit ocazia să trecem prin tot ciclul de viață al unei aplicații, de la arhitectură și baze de date, până la frontend și integrare AI. Am învățat nu doar tehnologii noi (.NET Core, Identity, Gemini API), ci și soft skills esențiale: lucrul în echipă, gestionarea conflictelor de cod (Git) și adaptarea la situații neprevăzute.
