import { test, expect } from "../fixtures/fixtures";
import { APP_URLS, POST_LOGIN_URL } from "../config/sites";

test('login tests', async ({ cmsAdminPage, gymAdminPage }) => {
  // arrange
  const url = APP_URLS['fithub'];
  
  const cmsUrl = `${url}/${POST_LOGIN_URL['cmsAdmin']}`;
  const gymUrl = `${url}/${POST_LOGIN_URL['gymAdmin']}`
  
  // act
  
  // assert
  await expect(cmsAdminPage).toHaveURL(cmsUrl);
  await expect(gymAdminPage).toHaveURL(gymUrl)
})