# Project Resonance Queer Communist Game Jam 2026
https://itch.io/jam/queercomgames2026

## 🛠️ How to Move Accidental Master Commits to a New Branch (Dev Workflow)

If you accidentally committed changes directly to your local `master` branch and GitHub blocks your push due to repository rulesets, follow these steps using **GitHub Desktop** to move your work to a new feature branch, merge it into `dev`, and safely clean up your local `master`.

### Step 1: Move Your Commits to a Feature Branch
1. Open **GitHub Desktop** and ensure your current branch dropdown is set to `master`.
2. Click on the **Current Branch** dropdown menu at the top.
3. Click the **New Branch** button.
4. Name your new feature branch and click **Create Branch**.
5. When a prompt appears asking what to do with your changes, select:
   * **"Bring my changes to [your-new-branch]"**
6. Click **Switch Branch** to confirm. 

*Your accidental commits are now safely attached to this new branch name.*

### Step 2: Reset Your Local Master Branch
Because your local `master` branch is still stuck ahead of the server, you need to roll it back so it matches the cloud.
1. Switch your current branch dropdown back to **`master`**.
2. Click on the **History** tab in the left sidebar (next to the "Changes" tab).
3. Scroll down to find the last commit that features an **origin/master** label or a small GitHub profile icon. This is the last commit currently alive on the server.
4. **Right-click** that specific commit and select **Reset to commit**.
5. Click back over to the **Changes** tab on the left sidebar.
6. **Right-click** anywhere inside the list of modified files and select **Discard all changes**. Your local `master` is now completely clean.

### Step 3: Fetch and Update the Dev Branch
Before working on your feature branch, you must ensure your local `dev` branch is completely up to date with the server.
1. Switch your current branch dropdown to **`dev`**.
2. Click the **Fetch origin** button at the top right. 
3. If the button changes to **Pull origin**, click it to pull down the latest updates from your team.

### Step 4: Rebase Your Feature Branch on Top of Dev
Now you will align your feature branch so that your work starts exactly where the latest `dev` branch ends.
1. Switch your current branch dropdown back to your **new feature branch**.
2. In the top menu bar, go to **Branch** -> **Rebase current branch...**.
3. Select **`dev`** from the list of branches.
4. Click **Rebase**. (If any merge conflicts appear, resolve them using your editor, then click *Continue Rebase*).

### Step 5: Merge Your Feature Branch Into Dev
Now that your feature branch is cleanly aligned with `dev`, you will merge your work directly into `dev` locally.
1. Switch your current branch dropdown to **`dev`**.
2. In the top menu bar, go to **Branch** -> **Merge into current branch...**.
3. Select your **new feature branch** from the list.
4. Click **Merge [your-feature-branch] into dev**.

### Step 6: Push Dev and Create Your Pull Request
1. While still on the **`dev`** branch, click the **Push origin** button in the top right corner to upload the newly merged commits to GitHub.
2. Click the **Create Pull Request** button to open your browser.
3. In GitHub, ensure your pull request base is set to **`master`** and your compare branch is set to **`dev`** to submit your code for final team review.
