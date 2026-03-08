import { chromium } from 'playwright';
import { writeFileSync, mkdirSync } from 'fs';
import { join } from 'path';

const BASE_URL = 'http://localhost:5173';
const LOGIN = 'alexcrazy42@mail.ru';
const PASSWORD = 'alexcrazy42';
const SCREENSHOTS_DIR = './test-screenshots';

mkdirSync(SCREENSHOTS_DIR, { recursive: true });

let stepNum = 0;
async function step(page, label) {
  stepNum++;
  const file = join(SCREENSHOTS_DIR, `${String(stepNum).padStart(2, '0')}-${label.replace(/[^a-z0-9]/gi, '_')}.png`);
  await page.screenshot({ path: file, fullPage: false });
  console.log(`[${stepNum}] ${label} -> ${file}`);
}

async function run() {
  const browser = await chromium.launch({ headless: false, slowMo: 300 });
  const context = await browser.newContext({ viewport: { width: 1400, height: 900 } });
  const page = await context.newPage();

  // ── 1. Open login page ──────────────────────────────────────────────────────
  await page.goto(`${BASE_URL}/login`);
  await page.waitForLoadState('networkidle');
  await step(page, 'login-page');

  // ── 2. Fill credentials ─────────────────────────────────────────────────────
  await page.fill('input[type="email"]', LOGIN);
  await page.fill('input[type="password"]', PASSWORD);
  await step(page, 'credentials-filled');

  // ── 3. Submit ────────────────────────────────────────────────────────────────
  await page.click('button:has-text("Войти")');
  await page.waitForTimeout(2000);
  await step(page, 'after-login-click');

  // Role selection may appear
  const roleSelectionVisible = await page.isVisible('text=Выберите роль');
  if (roleSelectionVisible) {
    console.log('Role selection appeared — picking first role');
    // CmsAdmin is usually the first radio
    const radios = page.locator('input[type="radio"]');
    const count = await radios.count();
    console.log(`  Found ${count} role(s)`);
    for (let i = 0; i < count; i++) {
      const val = await radios.nth(i).getAttribute('value');
      console.log(`  Role ${i}: ${val}`);
    }
    await radios.first().check();
    await step(page, 'role-selected');
    await page.click('button:has-text("Продолжить")');
    await page.waitForTimeout(2000);
    await step(page, 'after-role-continue');
  }

  // ── 4. Navigate to admin/chat ────────────────────────────────────────────────
  await page.goto(`${BASE_URL}/admin/chat`);
  await page.waitForLoadState('networkidle');
  await page.waitForTimeout(1500);
  await step(page, 'admin-chat-page');

  // ── 5. Click on a chat (first item in the list) ──────────────────────────────
  const chatItems = page.locator('.cursor-pointer').first();
  const chatItemVisible = await chatItems.isVisible().catch(() => false);
  if (chatItemVisible) {
    await chatItems.click();
    await page.waitForTimeout(1500);
    await step(page, 'chat-opened');
  } else {
    // Try alternative selectors
    const chatList = page.locator('[class*="chat"]').first();
    if (await chatList.isVisible().catch(() => false)) {
      await chatList.click();
      await page.waitForTimeout(1500);
    }
    await step(page, 'chat-list-state');
  }

  // ── 6. Find and click the sticker button (FileImageOutlined) ─────────────────
  // The sticker button is in the message input area
  await page.waitForTimeout(500);

  // Look for button near the text input in the message area
  // FileImageOutlined renders as an anticon-file-image span
  const stickerBtn = page.locator('button:has(.anticon-file-image)');
  const stickerBtnVisible = await stickerBtn.isVisible().catch(() => false);
  console.log(`Sticker button visible: ${stickerBtnVisible}`);

  if (stickerBtnVisible) {
    await stickerBtn.click();
    await page.waitForTimeout(1500);
    await step(page, 'sticker-picker-opened');

    // ── 7. Check what's inside the picker ──────────────────────────────────────
    const tabsVisible = await page.locator('.ant-tabs').isVisible().catch(() => false);
    const emptyVisible = await page.locator('text=Нет пакетов стикеров').isVisible().catch(() => false);
    const loadingVisible = await page.locator('.ant-spin').isVisible().catch(() => false);
    console.log(`  Tabs visible: ${tabsVisible}`);
    console.log(`  Empty state: ${emptyVisible}`);
    console.log(`  Loading: ${loadingVisible}`);

    if (tabsVisible) {
      // ── 8. Click first sticker ────────────────────────────────────────────────
      await page.waitForTimeout(800);
      const firstSticker = page.locator('.ant-popover-inner button img').first();
      const firstStickerVisible = await firstSticker.isVisible().catch(() => false);
      console.log(`First sticker image visible: ${firstStickerVisible}`);

      if (firstStickerVisible) {
        await firstSticker.click({ force: true });
        await page.waitForTimeout(2000);
        await step(page, 'sticker-sent');

        // ── 9. Verify sticker appears in the message list ─────────────────────
        const stickerInChat = page.locator('.object-contain').last();
        const stickerInChatVisible = await stickerInChat.isVisible().catch(() => false);
        console.log(`Sticker visible in chat: ${stickerInChatVisible}`);
        await step(page, 'sticker-in-chat-verification');
      } else {
        await step(page, 'no-stickers-in-picker');
        console.log('No sticker images found in picker - check if groups/stickers are seeded');
      }
    } else if (emptyVisible) {
      await step(page, 'empty-sticker-groups');
      console.log('No active sticker groups found. Create some in /admin/stickers first.');
    }
  } else {
    // Dump all buttons in the input area to help debug
    console.log('Sticker button NOT found. Dumping buttons in page:');
    const allButtons = page.locator('button');
    const btnCount = await allButtons.count();
    console.log(`  Total buttons: ${btnCount}`);
    for (let i = 0; i < Math.min(btnCount, 20); i++) {
      const cls = await allButtons.nth(i).getAttribute('class').catch(() => '');
      const txt = await allButtons.nth(i).innerText().catch(() => '');
      const icon = await allButtons.nth(i).locator('[class*="anticon"]').getAttribute('class').catch(() => '');
      console.log(`  [${i}] class="${cls.slice(0, 60)}" text="${txt.trim().slice(0, 30)}" icon="${icon}"`);
    }
    await step(page, 'sticker-button-not-found');
  }

  // ── 10. Check /admin/stickers page ──────────────────────────────────────────
  await page.goto(`${BASE_URL}/admin/stickers`);
  await page.waitForLoadState('networkidle');
  await page.waitForTimeout(1500);
  await step(page, 'admin-stickers-page');

  console.log('\n=== Test complete. Check screenshots in ./test-screenshots/ ===\n');
  await page.waitForTimeout(3000);
  await browser.close();
}

run().catch(err => {
  console.error('Test failed:', err);
  process.exit(1);
});
